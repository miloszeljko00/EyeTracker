"""
Author comments:
- If all sessions are not of equal lenght not many may be clustered,
  right now by default, not all sessions are equal length

- To introduce equal length sessions, one can clip all sessions
  to be the lenght of the shortest one
"""

from typing import Dict, List, Tuple
import yaml

import pandas as pd
import numpy as np
import numpy.typing as npt
import editdistance
from sklearn.cluster import DBSCAN

from time_per_region import get_roi_intervals


def read_sessions(sessions_path: str) -> List[str]:
    with open(sessions_path, "r+", encoding="utf-8") as _f:
        sessions = yaml.safe_load(_f)["sessions"]

    return sessions


def load_csv(path: str) -> pd.DataFrame:
    return pd.read_csv(path)


def get_session_name(path: str) -> str:
    # normalize paths to have only forward slash
    path = path.replace("\\", "/").replace("//", "/")

    return path.split("/")[-1].split(".csv")[0]


# threshold for noise is 200ms which is a 5th of a second which is 12 frames (since camera is 60fps)
# 200 because that's the trained human body reflex, avg is 250
# denoising done by not counting that roi at all in the sequence
def get_roi_sequence(
    session_df: pd.DataFrame, roi_intervals: List[Tuple[int, int]]
) -> List[str]:
    return [
        session_df.iloc[interval[0]]["Label"]
        for interval in roi_intervals
        if (interval[1] - interval[0]) > 12
    ]


def get_session_sequence(session_path: str) -> List[str]:
    session_df = load_csv(session_path)
    intervals = get_roi_intervals(session_df)

    return get_roi_sequence(session_df, intervals)


def get_all_session_sequences(sessions_yaml: str) -> List[Dict[str, List[str]]]:
    return {
        get_session_name(session): get_session_sequence(session)
        for session in read_sessions(sessions_yaml)
    }


def get_unique_rois(sequences: Dict[str, List[str]]) -> List[str]:
    rois = []
    for _, sequence in sequences.items():
        rois.extend(sequence)

    return np.unique(rois)


def make_roi_encoding_map(unique_rois: List[str]) -> Dict[str, str]:
    return dict(zip(unique_rois, np.arange(unique_rois.shape[0]).astype(np.str_)))


def encode_session_sequence(sequence: List[str], roi_map: Dict[str, str]) -> str:
    return "".join([roi_map[roi] for roi in sequence])


def encode_all_sessions(sequences: Dict[str, List[str]]) -> Dict[str, str]:
    unique_rois = get_unique_rois(sequences)
    roi_map = make_roi_encoding_map(unique_rois)

    return {
        session_name: encode_session_sequence(sequence, roi_map)
        for session_name, sequence in sequences.items()
    }


def precompute_distance_matrix(sequences: Dict[str, List[str]]) -> npt.NDArray[np.int_]:
    """
    Precomputes the distance matrix using edit distance
    Row (and Col) indexes are in order that sequences show up in
    """
    distance_matrix = np.empty(shape=(len(sequences), len(sequences)), dtype=np.int_)

    for row, sequence_row in enumerate(sequences.values()):
        for col, sequence_col in enumerate(sequences.values()):
            distance_matrix[row, col] = editdistance.eval(sequence_row, sequence_col)

    return distance_matrix


def clustering(distance_matrix: npt.NDArray[np.int_]) -> npt.NDArray[np.int_]:
    """
    Returns the clusters of each row of the matrix
    """
    dbscan = DBSCAN(eps=4, min_samples=2, metric="precomputed")

    return dbscan.fit_predict(distance_matrix)


def map_cluster_to_session(
    encoded_sequences: Dict[str, str], clusters: npt.NDArray[np.int_]
) -> Dict[str, np.int_]:
    return dict(zip(encoded_sequences, clusters))


def main():
    sequences = get_all_session_sequences("./sessions.yaml")
    encoded_sequences = encode_all_sessions(sequences)
    d_matrix = precompute_distance_matrix(encoded_sequences)
    clusters = clustering(d_matrix)
    clustered_sessions = map_cluster_to_session(encoded_sequences, clusters)
    print(clustered_sessions)


if __name__ == "__main__":
    main()

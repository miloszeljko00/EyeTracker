import argparse
import os

import pandas as pd
import numpy as np
import numpy.typing as npt
import matplotlib.pyplot as plt
from sklearn.cluster import DBSCAN


def load_csv(path: str) -> pd.DataFrame:
    return pd.read_csv(path)


def extract_coordinates(df: pd.DataFrame) -> npt.NDArray[np.float_]:
    return df[["BPOGX", "BPOGY"]].to_numpy(dtype=np.float_)


def clustering(coordinates: npt.NDArray[np.float_]) -> DBSCAN:
    dbscan = DBSCAN(eps=0.1, min_samples=3, metric="euclidean").fit(coordinates)

    return dbscan


def plot_and_save_clusters(
    dbscan: DBSCAN, coordinates: npt.NDArray[np.float_], path: str, session_name: str
) -> None:
    plt.scatter(coordinates[:, 0], coordinates[:, 1], c=dbscan.labels_, cmap="viridis")
    plt.colorbar()
    plt.title("Clustered (BPOGX, BPOGY) coordinates")
    plt.xlabel("BPOGX")
    plt.ylabel("BPOGY")
    plt.grid()

    plt.savefig(os.path.join(path, session_name + ".png"))


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument(
        "--csv_path", help="Path to csv from which the script will read"
    )
    parser.add_argument(
        "--save_path", help="Path to where the sequence data will be saved"
    )
    parser.add_argument("--session_name", help="Name of the recording session")

    args = parser.parse_args()

    df = load_csv(args.csv_path)
    coords = extract_coordinates(df)
    scan = clustering(coords)
    plot_and_save_clusters(scan, coords, args.save_path, args.session_name)


if __name__ == "__main__":
    main()
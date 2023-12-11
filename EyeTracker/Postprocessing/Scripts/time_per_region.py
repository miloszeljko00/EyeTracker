import argparse
import os
from typing import List, Tuple

import pandas as pd
import numpy as np
import matplotlib.pyplot as plt


def load_csv(path: str) -> pd.DataFrame:
    return pd.read_csv(path)


def get_roi_intervals(df: pd.DataFrame) -> List[Tuple[int, int]]:
    shifted_df = df.shift(periods=1)
    change_indexes = np.where(~(df["Label"] == shifted_df["Label"]))[0]
    change_indexes = np.array([*change_indexes, -1])

    intervals = [
        (change_indexes[i], change_indexes[i + 1])
        for i in range(change_indexes.shape[0] - 1)
    ]

    return intervals


def get_time_per_roi(
    df: pd.DataFrame, roi_intervals: List[Tuple[int, int]]
) -> List[Tuple[str, int]]:
    time_per_roi = []
    for interval in roi_intervals:
        roi = df.iloc[interval[0]]["Label"]
        time = df.iloc[interval[-1]]["TIME"] - df.iloc[interval[0]]["TIME"]

        time_per_roi.append((roi, time))

    return time_per_roi


def plot_and_save_roi_sequence(
    time_per_roi: List[Tuple[str, int]], path: str, session_name: str
) -> None:
    rois = [pair[0] for pair in time_per_roi]
    durations = [pair[1] / 10000.0 for pair in time_per_roi]
    positions = np.arange(len(rois))
    width = 0.5

    plt.bar(positions, durations, width=width, align="center")
    plt.xlabel("ROI")
    plt.ylabel("Duration [s]")
    plt.title("ROI Sequence")
    plt.xticks(positions, rois, rotation=90)
    plt.grid(visible=True)

    plt.savefig(os.path.join(path, session_name + ".png"), bbox_inches="tight")

    
def main():
    print("Started time per region script")
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
    ints = get_roi_intervals(df)
    tpr = get_time_per_roi(df, ints)
    plot_and_save_roi_sequence(tpr, args.save_path, args.session_name)
    print("Finished time per region script")


if __name__ == "__main__":
    main()

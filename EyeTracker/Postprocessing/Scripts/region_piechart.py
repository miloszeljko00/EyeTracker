import argparse
import os
from typing import Dict

import pandas as pd
import numpy as np
import matplotlib.pyplot as plt


def load_csv(path: str) -> pd.DataFrame:
    return pd.read_csv(path)


def get_regions_and_values(df: pd.DataFrame) -> Dict[str, int]:
    regions = df["Label"].unique()
    values = [np.sum(df["Label"] == region) for region in regions]

    return dict(zip(regions, values))


def plot_and_save_piechart(
    regions_and_values: Dict[str, int], path: str, session_name: str
) -> None:
    plt.pie(
        regions_and_values.values(), labels=regions_and_values.keys(), autopct="%1.1f%%"
    )
    plt.title("Share of time per ROI")
    plt.savefig(os.path.join(path, session_name + ".png"))


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument(
        "--csv_path", help="Path to csv from which the script will read"
    )
    parser.add_argument("--save_path", help="Path to where the piechart will be saved")
    parser.add_argument("--session_name", help="Name of the recording session")

    args = parser.parse_args()

    df = load_csv(args.csv_path)
    data = get_regions_and_values(df)
    plot_and_save_piechart(data, args.save_path, args.session_name)


if __name__ == "__main__":
    main()

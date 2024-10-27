import matplotlib.pyplot as plt
import pandas as pd
import argparse
import os

def main(filename):
   print("Filename: ", filename)
   data_identifier = os.path.basename(filename).split(".")[0]
   print(os.getcwd())
   print(data_identifier)
   with open(filename) as f:
      df = pd.read_csv(f)

   # actual usable data starts at row 25
   # df.iloc[25:]

   # count the states
   state_counts = {}
   for state in ["STATE_SMALL", "STATE_MED", "STATE_LARGE"]:
      state_counts[state] = df[df["Most Common State"] == state].shape[0]

   plt.bar(state_counts.keys(), state_counts.values())
   plt.xlabel("State")
   plt.ylabel("Count")
   plt.title("Most Common State among three states")
   plt.savefig(f"Python/AnalysisData/Graphs/Bar/{data_identifier}_MostCommonState.png")
   plt.clf()

   # same data with pie chart, add the number to the label text to another var to use as the labels var
   labels = [f"{state}: {count}" for state, count in state_counts.items()]
   plt.pie(state_counts.values(), labels=labels)
   plt.title("Most Common State")
   plt.savefig(f"Python/AnalysisData/Graphs/Pie/{data_identifier}_MostCommonState.png")
   plt.clf()


   respawn_counts = {}
   for i in range(1, 6):
      respawn_counts[f"C{i}"] = df.iloc[25:][f"Respawns at C{i}"].apply(int).sum()


   plt.bar(respawn_counts.keys(), respawn_counts.values())
   plt.xlabel("Checkpoint")
   plt.ylabel("Count")
   plt.title("Respawn Counts")
   plt.savefig(f"Python/AnalysisData/Graphs/Bar/{data_identifier}_RespawnCounts.png")
   plt.clf()


   labels = [f"{respawn}: {count}" for respawn, count in respawn_counts.items()]
   plt.pie(respawn_counts.values(), labels=labels)
   plt.title("Respawn Counts")

   plt.savefig(f"Python/AnalysisData/Graphs/Pie/{data_identifier}_RespawnCounts.png")
   plt.clf()

if __name__ == "__main__":
   argparser = argparse.ArgumentParser()
   argparser.add_argument("filename", type=str)
   args = argparser.parse_args()
   main(args.filename)

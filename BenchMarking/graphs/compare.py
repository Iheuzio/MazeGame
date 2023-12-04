import pandas as pd
import matplotlib.pyplot as plt

# Read the CSV files
csv_file_path = r"../data/final_results.csv"
df = pd.read_csv(csv_file_path)
csv_file_path1 = r"../data/results.csv"
df1 = pd.read_csv(csv_file_path1)

# Organize the data into dictionaries
series_data = {'Debug-HuntKill': {}, 'Debug-RecursiveMaze': {}, 'Release-HuntKill': {}, 'Release-RecursiveMaze': {}}
series_data1 = {'Debug-HuntKill': {}, 'Debug-RecursiveMaze': {}, 'Release-HuntKill': {}, 'Release-RecursiveMaze': {}}

# Function to update series_data with new DataFrame
def update_series_data(series_data, df):
    for index, row in df.iterrows():
        maze_type = row['MazeType']
        time_ms = row['Time (ms)']
        width = row['Width']
        height = row['Height']

        # Create a dictionary for the current row
        series_info = {'MazeType': maze_type, 'Time (ms)': time_ms, 'Width': width, 'Height': height}

        # Add the dictionary to the corresponding series in the dictionary
        if 'Debug' in maze_type:
            if 'Debug-HuntKill' in maze_type:
                maze_type = 'Debug-HuntKill'
            elif 'Debug-RecursiveMaze' in maze_type:
                maze_type = 'Debug-RecursiveMaze'
        elif 'Release' in maze_type:
            if 'Release-HuntKill' in maze_type:
                maze_type = 'Release-HuntKill'
            elif 'Release-RecursiveMaze' in maze_type:
                maze_type = 'Release-RecursiveMaze'

        series_dict = series_data[maze_type]

        if maze_type not in series_dict:
            series_dict[maze_type] = {'Width': [], 'Time (ms)': []}

        # Append data to the series
        series_dict[maze_type]['Width'].append(width)
        series_dict[maze_type]['Time (ms)'].append(time_ms)

# Update series_data with data from the first DataFrame
update_series_data(series_data, df)

# Update series_data1 with data from the second DataFrame
update_series_data(series_data1, df1)

# Create a figure and axis
fig, ax = plt.subplots(figsize=(10, 6))

# extract the last values from the series_data dictionaries
last_values = {}
last_values1 = {}
for maze_type, data in series_data.items():
    for label, sub_data in data.items():
        last_values[label] = sub_data['Time (ms)'][-1]

for maze_type, data in series_data1.items():
    for label, sub_data in data.items():
        last_values1[label] = sub_data['Time (ms)'][-1]

# Plot the bar chart for the last values
bar_width = 0.35
index = range(len(series_data))

bars_original = ax.bar(index, last_values1.values(), bar_width, label='Original')
bars_new = ax.bar([i + bar_width for i in index], last_values.values(), bar_width, label='New')

# attach performance label with % difference
for bar_original, bar_new in zip(bars_original, bars_new):
    height_original = bar_original.get_height()
    height_new = bar_new.get_height()
    difference = height_new - height_original
    difference_percent = difference / height_original * 100
    ax.text(bar_original.get_x() + bar_original.get_width() / 2.0, height_original, '%d%%' % difference_percent,
            ha='center', va='bottom')

# Set labels and title
ax.set_xlabel('Maze Type')
ax.set_ylabel('Time (ms)')
ax.set_title('Maze Generation Comparison Bar Chart')
ax.set_xticks([i + bar_width / 2 for i in index])
ax.set_xticklabels(last_values.keys())
ax.legend()

# Show the plot
plt.show()

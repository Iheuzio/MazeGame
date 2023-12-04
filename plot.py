import pandas as pd
import matplotlib.pyplot as plt

# Read the CSV file
csv_file_path = r"C:\Users\chris\Downloads\results_improvement.csv"
df = pd.read_csv(csv_file_path)

# Organize the data into a dictionary
series_data = {'Debug-HuntKill': {}, 'Debug-RecursiveMaze': {}, 'Release-HuntKill': {}, 'Release-RecursiveMaze': {}}

# Iterate over rows in the DataFrame
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

# Create a figure and axis
fig, ax = plt.subplots(figsize=(10, 6))

# Plot each series separately
for maze_type, data in series_data.items():
    for label, sub_data in data.items():
        ax.plot(sub_data['Width'], sub_data['Time (ms)'], label=label, marker='o')

# Set labels and title
ax.set_xlabel('Size of Maze (WxH)')
ax.set_ylabel('time_ms')
ax.set_title('Maze Generation Graph')
ax.legend()

# Set y-axis limits
ax.set_ylim(0, max(df['Time (ms)']) + 10)

# Show the plot
plt.show()

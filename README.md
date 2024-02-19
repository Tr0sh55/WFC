# Wave Function Collapse (WFC) Unity Implementation

## Overview

The WFC algorithm works by collapsing a superposition of possible tiles into a single state based on certain constraints. This implementation uses a grid of cells, where each cell starts with the possibility of becoming any tile. Through the process of observation (selecting a cell) and propagation (updating neighbors based on the selection), the algorithm gradually collapses all cells into a determined state, resulting in a fully generated layout.

## Features

- **Tile-Based Generation:** Uses a set of predefined tiles to generate layouts.
- **Customizable Grid:** Adjustable grid size to scale the generation area.
- **Delay Between Collapses:** Configurable delay to visualize the algorithm in action.
- **Reset and Regenerate:** Allows clearing the grid and regenerating the layout dynamically with `Space`.
- **Neighbor Priority:** After collapsing a cell, neighbors with the least possibilities are prioritized.

## How to Use

1. Add the `/Scripts/WafeFunctionCollapse.cs` to an empty GameObject in your Scene
2. **Setup Tile Prefabs:** Assign your tile prefabs to the `tilePrefabs` array in the inspector. These are the building blocks of your generated layout.
3. **Filler Tile:** Specify a filler tile prefab for cases where no valid tile can be placed.
4. **Grid Dimensions:** Set the `gridWidth` and `gridHeight` to define the size of the generation area.
5. **Tile Padding:** Adjust `tilePadding` to control the spacing between tiles.
6. **Collapse Delay:** Use `collapseDelay` to set the pause duration between successive collapses, which is helpful for visualizing the algorithm's progress.
7. **Define Tiles** Add the `TileDirectionSetter` script to each of your prefabs, specifying, in which direction another tile is compatible. 
8. **Generate:** Press the space bar at runtime to clear the grid and generate a new layout.

## Implementation Details

- The core of the algorithm is in the `ProcessCells` and `CollapseCell` methods, where cells are chosen based on the least number of possible states and collapsed to a definitive state, respectively.
- `ChooseCellWithLeastPossibilities` method selects the next cell to collapse, prioritizing neighbors of the last collapsed cell to encourage a more coherent and locally consistent layout.
- `UpdateNeighborPossibilities` updates the possible states of neighboring cells based on the constraints defined by the collapsed cell, ensuring the generated layout adheres to the specified rules.

## Requirements

- Unity 2019.4 LTS or newer recommended for compatibility.
- Basic understanding of Unity's GameObjects and Prefabs

# Minesweeper
Game dev proj 1

## **GitHub Actions**

Adding GitHub Actions YML to update reademe with commit message on every push

## **Added Pages And Timer**

Refactored the application from a single window app to single frame-multi pages allowing to show different pages to users like start menu, settings, game itself etc

this was done by using Frame control:
Frame is hosted in main window and pages can be rendered inside these frames and navigated allowing to switch between frames as well

moved the MineField creation into game page

added temporary useless timer counter on top of mine field

learned that using stack panel to host the textfield didnt align it properly, but using grip to host the textfield made it align properly

changed workflow to accept special charecters in commit message
decoding the special chareccters to properly display in the readme

## **Added cell click logic and flag**

• Created a CellData object to hold cell properties such as enabled, flagged, mine, etc.
• Attached the CellData object to each cell as its Tag property.
• Added left-click and right-click methods for every button.
• Left-click reveals the cell content (hidden during button setup).
• Right-click replaces the content with a flag PNG.
• Prevented revealing a cell if it is flagged.
• Fixed workflow issue where README was unnecessarily updated with duplicate values from a previous merge when a branch was published.

## **Added Mines**

• Generated 10 random numbers between 0–99 to determine mine positions.
• Placed a mine if the cell number matches a generated number.
• Added temporary debug display: ‘1’ for cells with mines, empty string for cells without mines.
• Implemented mine counter display.
• Mine counter increments when a flag is placed and decrements when a flag is removed.
• Investigated issue where pull request merge and branch publish still trigger workflow to update README with empty message, added fix for testing.

## ****




## **Added FloodFill**

* **Modified  logic**

  * If the cell is a mine → show mine icon.
  * If the cell has an adjacent mine count > 0 → show the number and color it using .
  * If the cell has  → trigger  flood fill logic.

* **Implemented  (flood fill)**

  * Uses a **queue** () for BFS (Breadth-First Search) expansion.
  * Tracks visited cells using a **** to avoid duplicates and infinite loops.
  * Starts by enqueueing the initially clicked empty cell.
  * For each dequeued cell:

    * Disables the button and marks cell as revealed.
    * If it’s a numbered cell → reveal the number and stop expanding from it.
    * If it’s an empty cell → reveal it and enqueue all adjacent non-mine, unrevealed neighbors.
  * Expansion continues until no more cells can be revealed.

* **Added  helper**

  * Finds and returns the  UI element associated with a given  object.
  * Used in  to update the button's content and state during flood fill.

* **UI improvement for mine count**

  *  assigns different colors for each number (1 → Blue, 2 → Green, etc.) to match classic Minesweeper visuals.

* **Fixed unwanted publish trigger on branch creation**

  * Added  to the publish job in the GitHub Actions workflow.
  * This ensures the publish step only runs on **branch updates**, not when the branch is first created (which previously caused the wrong commit message to be picked up).

## **Update README.md**




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

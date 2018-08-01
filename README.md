# Crossword

Crossword game in Unity for a programming test.

Current progress:
Word List Database and Editing: DONE
Crossword generation: DONE
Game Interface: DONE

Build of the game can be found in the Build folder. Download all files in the build folder in order for the executable to be run.

Algorithm used for crossword generation:
1. Pick one word as the starting word and place it on the board. Vertical or Horizontal orientation is randomized.

2. Starting from the next word onwards, the following is done:

	a. Select a word from the board to start from (i.e. every word must intersect at least 1 word)

		i. Orient new word to be perpendicular with possible start-from word.

		ii. Check through all possible valid intersections with the word on the board

			1. As words must be place “top-down” or “left-right”, the current check will terminate if the current word intersection will move out of the left or top corner of the board to save on iterations.

		iii. For each valid intersection, check to make sure that the new word will not:

			1. Intersect incorrectly with any other word already on the board

			2. Right beside a word that is parallel to it (i.e. 1 block spacing between each word)

		iv. If the valid intersection can be legally placed, save the current placement and compute a score for it where the higher number of legal intersections, the better.

	b. Repeat step (a) for all words on the board to get all possible placements and return the placement that has the best score.

3. Place the word onto the board using the best placement generated (i.e. highest score)

4. Once all words are placed, check if this board’s score is higher than the previous attempt’s score. If it is, use the new board.

5. Iterate until maximum amount of tries attempted (to go through as many possible permutations of boards to get the board with the best score) and return the generated board.

Generation code can be found in Scripts/Crossword/LevelGenerator.cs
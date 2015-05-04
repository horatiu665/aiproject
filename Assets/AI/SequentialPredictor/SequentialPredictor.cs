using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SequentialPredictor
{

	/// <summary>
	/// Returns the number that is predicted to most likely follow after the list, based on sequences found in the list that are similar to the sequence of the last N numbers.
	/// </summary>
	/// <param name="list">list of numbers, representing the history of performed actions, each action being mapped as an integer from 0 to N</param>
	/// <param name="alphabetSize">amount of possible actions</param>
	/// <returns></returns>
	public static int PredictNext(List<int> list, int alphabetSize, int historySize = 500)
	{
		// search for the longest sequence of elements in list which is also found somewhere else in list.
		// the next element of the recognized sequence is the most likely to appear again in the sequence.
		// assign scores to the predictions and return the one with the highest score.
		//		score based on length of found sequence, and on amount of sequences found.

		// historySize limits the search space
		if (list.Count > historySize && historySize > 0) {
			list = list.GetRange(list.Count - historySize, historySize);
		}

		int maxSequenceLength = 10 > list.Count / 2 ? list.Count / 2 : 10;
		List<float> alphabetScores = new List<float>(alphabetSize);
		for (int i = 0; i < alphabetSize; i++) {
			alphabetScores.Add(0);
		}

		for (int currentLength = 1; currentLength < maxSequenceLength; currentLength++) {

			// search from first element to the last list that might repeat itself.
			for (int searchPos = 0; searchPos <= list.Count - currentLength * 2; searchPos++) {

				if (CompareSequencesInList(list, searchPos, list.Count - currentLength, currentLength)) {
					// equal sequence found!

					// we must take the next element of each equalSequence and score that element in the alphabetScores.
					// at the end, alphabetScores will contain values representing how many times that element was a probable occurence after an arbitrary sequence.
					var probablePrediction = list[searchPos + currentLength];

					// longer sequences are more valuable predictions.
					alphabetScores[probablePrediction] += GetPredictionScore(currentLength, searchPos / (float)(list.Count));

				}
			}

		}

		// now we have the scores in the alphabetScores list, time to decide which one to consider the most likely to be next.

		// the highest score in the predictionScores list is the most accurate prediction.
		var maxScoreIndex = 0;
		for (int i = 0; i < alphabetSize; i++) {
			if (alphabetScores[i] > alphabetScores[maxScoreIndex]) {
				maxScoreIndex = i;
			}
		}

		// maxScoreIndex now holds the number with the highest chance of being the next in the sequence.
		return maxScoreIndex;

	}

	/// <summary>
	/// returns true when sublists of length sequenceLength from indices pos1 and pos2 of list are identical.
	/// </summary>
	private static bool CompareSequencesInList(List<int> list, int pos1, int pos2, int sequenceLength)
	{
		for (int i = 0; i < sequenceLength; i++) {
			if (list[pos1 + i] != list[pos2 + i]) {
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// used to calculate scores of alphabet options, by taking equalSequence length into account (if a small sequence is found, the score should be lower than for a long sequence).
	/// </summary>
	/// <param name="sequenceLength">length of sequence being compared with</param>
	/// <returns></returns>
	private static float GetPredictionScore(int sequenceLength, float searchPos)
	{
		// searchPos => the more recent it is, the higher the score.
		return sequenceLength / 3f + searchPos * sequenceLength / 3f;
	}
}

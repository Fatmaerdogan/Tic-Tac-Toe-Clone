using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameAreaPanel : MonoBehaviour {

    [SerializeField]
    private List<Button> buttons = new List<Button>();

    private bool turn;
    private int fieldsLeft;
    private int optimalScoreButtonIndex = -1;

    private void Start()=>Events.OnGameStart += StartGame;
    private void OnDestroy()=>Events.OnGameStart -= StartGame;

    public void StartGame() {
        turn = Mathf.Round(UnityEngine.Random.Range(0, 1)) == 1;
        Reset();
    }
    private void EnableButtons(bool enabled, bool ignoreEmpty = false) {
        foreach (Button button in buttons) {
            if (!enabled || ignoreEmpty || IsFieldEmpty(button))button.interactable = enabled;
        }
    }
    private bool IsFieldEmpty(Button button) {return GetText(button).text == "";}
    private Text GetText(Button button) {return button.GetComponentInChildren<Text>();}
    private bool SetMarkAndCheckForWin(Button button, bool colorate = false) {
        Text text = GetText(button);
        if (text.text != "") return false;
        text.text = turn ? "X" : "O";
        fieldsLeft--;
        return CheckForWin(text.text, colorate);
    }

    public void OnButtonClick(Button button) {
        if (SetMarkAndCheckForWin(button, true)) {
            Win();
            return;
        }
        button.interactable = false;
        if (fieldsLeft <= 0) Over();
        turn = !turn;
        if ( fieldsLeft > 0 && !turn)StartCoroutine(AiTurnCoroutine());
    }
    private IEnumerator AiTurnCoroutine() {
        EnableButtons(false);
        IEnumerator minMaxEnumerator = MinMaxCoroutine(1);
        yield return new WaitForSeconds(0.2f);
        while (minMaxEnumerator.MoveNext()) {}
        Button button = buttons[optimalScoreButtonIndex];
        EnableButtons(true);
        OnButtonClick(button);

    }
    private IEnumerator MinMaxCoroutine(int depth) {
        if (CheckBaseCaseAndShortcuts()) {
            yield break;
        }
        int currentBestScore = turn ? Int32.MinValue : Int32.MaxValue;
        int currentOptimalScoreButtonIndex = -1;
        int fieldIndex = 0;
        while (fieldIndex < buttons.Count) {
            if (IsFieldFree(fieldIndex)) {
                Button button = buttons[fieldIndex];
                int currentScore = 0;

                bool endRecursion = false;
                if (SetMarkAndCheckForWin(button)) {
                    currentScore = (turn ? 1 : -1) * (10 - depth);
                    endRecursion = true;
                } else if (fieldsLeft > 0) {
                    turn = !turn; 
                    IEnumerator minMaxEnumerator = MinMaxCoroutine(depth + 1);
                    while (minMaxEnumerator.MoveNext()) { }
                    turn = !turn; 
                }

                if ((turn && currentScore > currentBestScore) || (!turn && currentScore < currentBestScore)) {
                    currentBestScore = currentScore;
                    currentOptimalScoreButtonIndex = fieldIndex;
                }
                GetText(button).text = "";
                fieldsLeft++;

                if (endRecursion) break;
            }
            fieldIndex++;
        }
        optimalScoreButtonIndex = currentOptimalScoreButtonIndex;
    }
    private bool CheckBaseCaseAndShortcuts() {
        if (fieldsLeft <= 0) return true;
        if (fieldsLeft == 9) {
            RandomCorner();
            return true;
        }
        if (fieldsLeft == 8) {
            if (!GetText(buttons[4]).text.Equals(""))RandomCorner();
            else optimalScoreButtonIndex = 4;
            return true;
        }
        return false;
    }
    private bool CheckForWin(string mark, bool colorate = false) {
        if (fieldsLeft > 6) return false;
        if (CompareButtons(0, 1, 2, mark, colorate)|| CompareButtons(3, 4, 5, mark, colorate)|| CompareButtons(6, 7, 8, mark, colorate)|| CompareButtons(0, 3, 6, mark, colorate)|| CompareButtons(1, 4, 7, mark, colorate)|| CompareButtons(2, 5, 8, mark, colorate)|| CompareButtons(0, 4, 8, mark, colorate)|| CompareButtons(6, 4, 2, mark, colorate)) return true;
        return false;
    }

    private bool CompareButtons(int ind1, int ind2, int ind3, string mark, bool colorate = false) {
        Text text1 = GetText(buttons[ind1]);
        Text text2 = GetText(buttons[ind2]);
        Text text3 = GetText(buttons[ind3]);
        bool equal = text1.text == mark
                  && text2.text == mark
                  && text3.text == mark;
        if (colorate && equal) {
            Color color = turn ? Color.green : Color.red;
            text1.color = color;
            text2.color = color;
            text3.color = color;
        }
        return equal;
    }

    private bool IsFieldFree(int index) => GetText(buttons[index]).text.Length == 0;
    private void Win() {
        EnableButtons(false);
        Events.OnGameOver?.Invoke(turn ? 0 : 1);
    }
    private void Over() {
        EnableButtons(false);
        Events.OnGameOver?.Invoke(-1);
    }
    private int RandomCorner() {
        optimalScoreButtonIndex = (int)Mathf.Floor(UnityEngine.Random.Range(0, 4));
        if (optimalScoreButtonIndex == 1) {
            optimalScoreButtonIndex = 6;
        } else if (optimalScoreButtonIndex == 3) {
            optimalScoreButtonIndex = 8;
        }
        return optimalScoreButtonIndex;
    }
    private void Reset() {
        foreach (Button button in buttons) {
            Text text = GetText(button);
            text.color = Color.grey;
            text.text = "";
            button.interactable = true;
        }
        fieldsLeft = 9;
        if (!turn) {
            StartCoroutine(AiTurnCoroutine());
        }
    }
}

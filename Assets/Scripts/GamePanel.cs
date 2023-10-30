using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour {
    [SerializeField]
    private Text player1Text;
    [SerializeField]
    private Text player2Text;
    [SerializeField]
    private Button startButton;
    private int p1Score = 0;
    private int p2Score = 0;

    void Start()=>Events.OnGameOver += OnGameOver;
    void OnDestroy()=>Events.OnGameOver -= OnGameOver;
    public void OnStartClicked() {
        StartButtonInteractable();
        Events.OnGameStart?.Invoke();
    }

    public void OnGameOver(int win) {
        if (win == 0) {
            p1Score++;
            player1Text.text = "Player 1 \nWin : " + p1Score;
        } else if(win==1){
            p2Score++;
            player2Text.text = "Player 2 \nWin : " + p2Score;
        }

        StartButtonInteractable();
    }
    public void StartButtonInteractable() => startButton.interactable = !startButton.interactable;
}

using System;
using UnityEngine;

public class BlackjackManager : MonoBehaviour {
    [SerializeField] private HoverHandler hitHover;
    [SerializeField] private HoverHandler standHover;
    [SerializeField] private HoverHandler splitHover;

    private readonly CardDeck deck = new();

    private readonly BlackJackStack firstStack = new();
    private readonly BlackJackStack secondStack = new();
    private readonly BlackJackStack dealerStack = new();

    private BlackJackStack currentStack;

    private bool hasSplit = false;

    private void Start() {
        deck.CreateDeck();

        firstStack.Reset();
        secondStack.Reset();
        dealerStack.Reset();

        currentStack = firstStack;
        firstStack.isActive = true;
        hasSplit = false;

        currentStack.Hit(deck.PickCard());
        currentStack.Hit(deck.PickCard());

        dealerStack.Hit(new Card(CardValues.FaceDown, CardSuit.FaceDown));
        dealerStack.Hit(deck.PickCard());
    }

    private void Update() {
        if (currentStack.CalculateScore() > 21) {
            currentStack.hasBust = true;

            if (!secondStack.isActive) {
                // end game here -> run next sequence
                return;
            }
            else {
                CheckForBust();
                return;
            }
        }

        if (secondStack.isActive)
            if (currentStack.hasStood)
                Switch();

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            if (hitHover.IsHovering) {
                currentStack.Hit(deck.PickCard());
                print("AAAA");
            }
            else if (standHover.IsHovering) {
                Stand();
            }
            else if (splitHover.IsHovering) {
                if (hasSplit)
                    Switch();
                else
                    Split();
            }
        }
    }

    private void Stand() {
        currentStack.Stand();

        if (secondStack.isActive) {
            CheckForBust();
            return;
        }

        int playerScore = firstStack.CalculateScore();
        int dealerScore = dealerStack.CalculateScore();

        dealerStack.ReplaceCard(0, deck.PickCard());
        DealerSequence(playerScore);

        Debug.Log($" Dealer scored: {dealerScore}");
        Debug.Log($" Your score: {playerScore}\n");

        if (dealerScore >= playerScore && dealerScore < 22)
            Debug.Log("  YOU LOST");
        else
            Debug.Log($"  YOU WIN!");
    }

    private void Split() {
        if (hasSplit)
            return;

        (bool, Card) canSplit = firstStack.OutputOnSplit();
        if (!canSplit.Item1)
            return;

        secondStack.isActive = true;

        firstStack.Hit(deck.PickCard());
        secondStack.Hit(canSplit.Item2);
        secondStack.Hit(deck.PickCard());

        hasSplit = true;
    }

    private void Switch() {
        if (!hasSplit)
            return;

        if (currentStack == firstStack)
            currentStack = secondStack;
        else
            currentStack = firstStack;
    }

    // Dealer drawing cards and trying to win!
    private void DealerSequence(int highestPlayerScore) {
        while (true) {
            if (dealerStack.CalculateScore() >= highestPlayerScore)
                break;

            dealerStack.Hit(deck.PickCard());
            DealerSequence(highestPlayerScore);
        }
    }

    private void CheckForBust() {
        if (currentStack == firstStack) {
            if (!firstStack.hasBust)
                firstStack.hasStood = true;
        }
        else {
            if (!secondStack.hasBust)
                secondStack.hasStood = true;
        }

        if ((firstStack.hasStood || firstStack.hasBust) &&
            (secondStack.hasStood || secondStack.hasBust)) {
            int highestScore = firstStack.CalculateScore() > secondStack.CalculateScore() ? firstStack.CalculateScore() : secondStack.CalculateScore();
            dealerStack.ReplaceCard(0, deck.PickCard());
            DealerSequence(highestScore);
            int dealerScore = dealerStack.CalculateScore();

            if (firstStack.hasStood) {
                int playerScore = firstStack.CalculateScore();
                Debug.Log($" Your score on Stack one: {playerScore}");

                if (dealerScore >= playerScore && dealerScore < 22)
                    Debug.Log("  YOU LOST on Stack one");
                else
                    Debug.Log($"  YOU WIN! On Stack one");
            }
            else
                Debug.Log("   BUST: YOU LOSE on Stack one!");

            if (secondStack.hasStood) {
                int playerScore = secondStack.CalculateScore();
                Debug.Log($" Your score on Stack two: {playerScore}");

                if (dealerScore >= playerScore && dealerScore < 22)
                    Debug.Log("  YOU LOST on Stack two");
                else {
                    Debug.Log($"  YOU WIN! On Stack two");
                }
            }
            else
                Debug.Log("   BUST: YOU LOSE on Stack two!");
        }
        else
            Switch();
    }
}
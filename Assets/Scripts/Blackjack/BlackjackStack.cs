using System.Collections.Generic;

public class BlackJackStack {
    public bool isActive = false;
    public bool hasStood = false;
    public bool hasBust = false;

    private readonly List<Card> stack = new();

    public void Reset() {
        isActive = false;
        hasStood = false;
        hasBust = false;

        stack.Clear();
    }

    public void ReplaceCard(int index, Card card) {
        stack[index] = card;
    }

    public void Stand() {
        hasStood = true;
    }

    public void Hit(Card cardToAdd) {
        stack.Add(cardToAdd);
    }

    public (bool, Card) OutputOnSplit() {
        if (stack.Count > 2)
            return (false, new Card(CardValues.FaceDown, CardSuit.FaceDown));

        if (stack[0].value == stack[1].value && stack[0].suit != stack[1].suit) {
            CardValues value = stack[1].value;
            CardSuit suit = stack[1].suit;
            stack.RemoveAt(1);
            return (true, new Card(value, suit));
        }

        return (false, new Card(CardValues.FaceDown, CardSuit.FaceDown));
    }

    public bool HasWon(int scoreCompare) {
        int ownScore = CalculateScore();

        if (hasBust || !isActive)
            return false;

        if (scoreCompare < ownScore && ownScore < 22)
            return true;

        return false;
    }

    public int CalculateScore() {
        int score = 0;
        int aceAmount = 0;

        foreach (Card card in stack) {
            int value = (int)card.value;
            if (value > 10)
                value = 10;
            else if (value == 1) {
                aceAmount++;
                continue;
            }
            score += value;
        }

        for (int i = 0; i < aceAmount; i++) {
            if (score + 11 > 21)
                score += 1;
            else
                score += 11;
        }

        return score;
    }
}

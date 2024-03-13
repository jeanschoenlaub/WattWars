using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Question Collection", menuName = "Quiz/Question", order = 0)]
public class QuestionSO : ScriptableObject
{
    public string questionText;
    public List<string> answers;
    public int correctAnswerIndex;
}
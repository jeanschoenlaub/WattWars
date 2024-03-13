using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Question Collection", menuName = "Quiz/QuestionCollection", order = 0)]
public class QuestionCollection : ScriptableObject
{
    public List<QuestionSO> questions = new List<QuestionSO>();
}
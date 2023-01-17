using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

public class UIButtonHandler : MonoBehaviour
{
    [SerializeField] private Cat cat;
    [SerializeField] private string groupTag = "";
    [SerializeField] private float baseCost; 
    [SerializeField] private float increaseAmount;

    private UnityEvent OnButtonPressed;
    private float cost;
    private float currentContribution;
    private VisualElement buyGroup;

    void Awake()
    {
        OnButtonPressed = new UnityEvent();
        OnButtonPressed.AddListener(Inc);

        if (cat == null) cat = GameObject.Find("Cat").GetComponent<Cat>();

        currentContribution = 0;
        cost = baseCost;
        buyGroup = this.gameObject.GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>(groupTag);
        buyGroup.Q<Button>("Buy").RegisterCallback<ClickEvent>((ev) => OnButtonPressed?.Invoke());
        if(buyGroup.Q<Label>("Name").text != groupTag) buyGroup.Q<Label>("Name").text = groupTag;
        updateText();
    }

    void updateText()
    {
        buyGroup.Q<Button>("Buy").text = $"Cost: {cost}";
        buyGroup.Q<Label>("PPS").text = $"Current PPS: {currentContribution}";
    }

    void Inc()
    {
        if (cat.IncrementPPS(cost, increaseAmount))
        {
            cost += baseCost * .1f;
            currentContribution += increaseAmount;
            updateText();
        }
    }
}

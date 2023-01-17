using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Cat : MonoBehaviour
{

    [SerializeField] private UIDocument meowsDocument;
    [SerializeField] private Sprite eyesOpen;
    [SerializeField] private Sprite eyesClosed;

    private float meows;
    private Label meowsText;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private float PPS;

    // Start is called before the first frame update
    void Start()
    {
        meows = 0;
        if (meowsDocument == null) meowsDocument = GameObject.Find("Meows").GetComponent<UIDocument>();
        meowsText = meowsDocument.rootVisualElement.Q<Label>("Meows");
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        audioSource = this.gameObject.GetComponent<AudioSource>();
        updateText();
    }

    void OnMouseDown()
    {
        meows += 1;
        updateText();

        audioSource.Stop();
        audioSource.time = 0;
        audioSource.Play();
        StartCoroutine(blink());
    }

    void Update()
    {
        meows += PPS * Time.deltaTime;
        updateText();
    }

    void updateText()
    {
        meowsText.text = $"Meows: {Math.Round(meows, 2)}\nPets Per Second: {Math.Round(PPS, 2)}";
    }

    IEnumerator blink() {
        spriteRenderer.sprite = eyesClosed;
        yield return new WaitForSeconds(.5f);
        spriteRenderer.sprite = eyesOpen;
    }

    public bool IncrementPPS(float cost, float amount)
    {
        if (cost <= meows)
        {
            PPS += amount;
            meows -= cost;
            return true;
        }
        return false;
    }
}

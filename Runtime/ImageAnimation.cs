using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageAnimation : MonoBehaviour
{
    [SerializeField] private ImageAnimSequence animationSequence;
    [SerializeField] private bool isAutoPlay = false;
    [SerializeField] private bool isLoop = false;

    private Image image;

    private float frameDuration;
    private int totalFrames;
    private bool isPlaying = false;
    private float playbackPercentage = 1f;
    private float elapsedTime;
    private int startFrameIndex;
    private int endFrameIndex;
    private bool hasStarted = false;


    // Start is called before the first frame update
    void OnEnable()
    {
        image = gameObject.GetComponent<Image>();

        if (animationSequence != null)
        {
            frameDuration = 1f / animationSequence.TargetFPS;
            totalFrames = animationSequence.AnimationSequence.Count;

            if (isAutoPlay)
            {
                Play(1f);
            }
        }

    }


    public void PlayPercent(int percentage) => Play((float)percentage / 100f);

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= frameDuration)
            {
                elapsedTime = 0f;

                if (hasStarted)
                {
                    startFrameIndex++;

                    if (startFrameIndex > endFrameIndex)
                    {
                        if (isLoop)
                        {
                            startFrameIndex = Mathf.FloorToInt(totalFrames * 0);
                            elapsedTime = 0f;
                        }
                        else
                        {
                            isPlaying = false;
                            startFrameIndex = endFrameIndex;
                        }
                    }
                }
            }
            else
            {
                hasStarted = true;
            }

            if (hasStarted)
            {
                image.sprite = animationSequence.AnimationSequence[startFrameIndex];
            }
        }
    }

    public void Play(float percentage)
    {
        if (animationSequence == null || percentage <= 0f) return;

        if (!isPlaying)
        {
            elapsedTime = 0f;
            hasStarted = false;
            isPlaying = true;
        }

        playbackPercentage = Mathf.Clamp01(percentage);

        int currentFrameIndex = animationSequence.AnimationSequence.IndexOf(image.sprite);

        if (currentFrameIndex < 0)
        {
            currentFrameIndex = 0;
        }

        startFrameIndex = currentFrameIndex;
        endFrameIndex = Mathf.FloorToInt(totalFrames * playbackPercentage) - 1;

        if (endFrameIndex >= totalFrames) endFrameIndex = totalFrames - 1;

        image.sprite = animationSequence.AnimationSequence[startFrameIndex];
    }

    private void ShowFrame(int frameNumber)
    {
        image.sprite = animationSequence.AnimationSequence[frameNumber];
        // currentFrame++;
    }
}

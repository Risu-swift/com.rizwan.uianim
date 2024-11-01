using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageAnimation : MonoBehaviour
{
    public List<ImageAnimSequence> animationSequence;
    public bool isAutoPlay = false;
    public bool isLoop = false;

    private Image image;

    private float frameDuration;
    private int totalFrames;
    private bool isPlaying = false;
    private float playbackPercentage = 1f;
    private float elapsedTime;
    private int currentFrameIndex;
    private int sequenceIndex = 0;
    private int targetFrameIndex;
    private bool hasStarted = false;

    public UnityEvent OnAnimationEnd;

    // Start is called before the first frame update
    void OnEnable()
    {
        image = gameObject.GetComponent<Image>();

        if (animationSequence != null && animationSequence.Count > 0)

        {

            if (isAutoPlay)
            {
                Play(1f);
            }
            else
            {
                image.sprite = animationSequence[0].AnimationSequence[0];
            }
        }

    }

    void OnDisable()
    {
        ResetAnimation();
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
                    currentFrameIndex++;

                    if (currentFrameIndex > targetFrameIndex)
                    {
                        if (sequenceIndex < animationSequence.Count - 1)
                        {
                            sequenceIndex++;
                            StartNewSequence();
                        }

                        else
                        {
                            if (isLoop)
                            {
                                currentFrameIndex = 0;
                                elapsedTime = 0f;
                                OnAnimationEnd.Invoke();
                            }
                            else
                            {
                                isPlaying = false;
                            }
                        }
                    }
                }
            }
            else
            {
                hasStarted = true;
            }

            if (currentFrameIndex >= 0 && currentFrameIndex < totalFrames)
            {
                image.sprite = animationSequence[sequenceIndex].AnimationSequence[currentFrameIndex];
            }
        }
    }

    public void Play(float percentage)
    {
        if (animationSequence == null || animationSequence.Count == 0 || percentage <= 0f) return;

        if (!isPlaying)
        {
            elapsedTime = 0f;
            hasStarted = false;
            isPlaying = true;
        }

        playbackPercentage = Mathf.Clamp01(percentage);

        var currentSequence = animationSequence[sequenceIndex];
        frameDuration = 1f / currentSequence.TargetFPS;
        totalFrames = currentSequence.AnimationSequence.Count;
        targetFrameIndex = Mathf.FloorToInt(totalFrames * playbackPercentage) - 1;

        if (targetFrameIndex >= totalFrames) targetFrameIndex = totalFrames - 1;

        if (!hasStarted)
        {
            currentFrameIndex = Mathf.Clamp(currentFrameIndex, 0, targetFrameIndex);
            image.sprite = currentSequence.AnimationSequence[currentFrameIndex];
        }
        else
        {
            if (currentFrameIndex <= targetFrameIndex)
            {
                StartPlaybackFromCurrentFrame(targetFrameIndex);
            }
        }
    }

    private void StartNewSequence()
    {
        var currentSequence = animationSequence[sequenceIndex];
        frameDuration = 1f / currentSequence.TargetFPS;
        totalFrames = currentSequence.AnimationSequence.Count;
        currentFrameIndex = 0;
        image.sprite = currentSequence.AnimationSequence[currentFrameIndex];
    }

    private void StartPlaybackFromCurrentFrame(int targetFrameIndex)
    {
        currentFrameIndex = Mathf.Clamp(currentFrameIndex, 0, targetFrameIndex);
        image.sprite = animationSequence[sequenceIndex].AnimationSequence[currentFrameIndex];
        hasStarted = true;
    }

    private void ResetAnimation()
    {
        isPlaying = false;
        hasStarted = false;
        elapsedTime = 0f;
        playbackPercentage = 0f;
        currentFrameIndex = 0;
        targetFrameIndex = 0;
        sequenceIndex = 0;

        if (animationSequence != null && animationSequence.Count > 0)
        {
            var initialSequence = animationSequence[0];
            image.sprite = initialSequence.AnimationSequence[0];
            frameDuration = 1f / initialSequence.TargetFPS;
            totalFrames = initialSequence.AnimationSequence.Count;
        }
    }
}

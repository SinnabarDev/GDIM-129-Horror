using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoSceneLoader : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer videoPlayer;

    [SerializeField]
    private string videoFileName;

    [SerializeField]
    private string nextScene;

    private void Start()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        string videoPath = Path.Combine(Application.streamingAssetsPath, videoFileName);

        Debug.Log("Video Path: " + videoPath);

        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = videoPath;

        videoPlayer.loopPointReached += EndReached;
        videoPlayer.Play();
    }

    private void EndReached(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextScene);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (videoPlayer == null)
            return;

        if (hasFocus)
            videoPlayer.Play();
        else
            videoPlayer.Pause();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (videoPlayer == null)
            return;

        if (pauseStatus)
            videoPlayer.Pause();
        else
            videoPlayer.Play();
    }
}
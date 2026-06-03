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
}

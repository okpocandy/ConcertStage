using UnityEngine;
using UnityEngine.Playables;
using OscJack;

public class OscTimelineSync : MonoBehaviour
{
    public PlayableDirector director;
    public int oscPort = 9000;
    
    public string addressTime = "/timeline/time";
    public string addressPlay = "/timeline/play";

    private OscServer _server;
    
    private bool _isPlaying = false;
    private bool _stateChanged = false;
    private float _currentTime = 0f; 
    private float _lastReceivedTime = -1f;
    private bool _needsSeek = false;

    void Start()
    {
        if (director != null)
        {
            director.timeUpdateMode = DirectorUpdateMode.Manual;
        }

        _server = new OscServer(oscPort);
        _server.MessageDispatcher.AddCallback(addressTime, OnReceiveTime);
        _server.MessageDispatcher.AddCallback(addressPlay, OnReceivePlay);
    }

    // 1. 슬라이더를 움직였을 때 (OSC Thread에서 콜백)
    void OnReceiveTime(string address, OscDataHandle data)
    {
        float incomingTime = data.GetElementAsFloat(0);
        
        if (Mathf.Abs(incomingTime - _lastReceivedTime) > 0.01f)
        {
            _lastReceivedTime = incomingTime;
            _currentTime = incomingTime; 
            _needsSeek = true;
        }
    }

    // 2. Play/Pause 버튼을 눌렀을 때 (OSC Thread에서 콜백)
    void OnReceivePlay(string address, OscDataHandle data)
    {
        float playValue = data.GetElementAsFloat(0); 
        bool targetState = (playValue > 0.5f);
        
        if (_isPlaying != targetState)
        {
            _isPlaying = targetState;
            _stateChanged = true;
        }
    }

    // 3. 매 프레임 업데이트 (Main Thread에서 안전하게 실행)
    void Update()
    {
        if (director == null) return;

        // 재생/일시정지 상태 변경 처리
        if (_stateChanged)
        {
            _stateChanged = false;
            if (_isPlaying)
            {
                // Mode가 다르면 DSPClock으로 바꾸고 그래프 재빌드 후 재생
                if (director.timeUpdateMode != DirectorUpdateMode.DSPClock)
                {
                    director.timeUpdateMode = DirectorUpdateMode.DSPClock;
                    director.RebuildGraph();
                }
                director.time = _currentTime;
                director.Play();
            }
            else
            {
                director.Pause();
                if (director.timeUpdateMode != DirectorUpdateMode.Manual)
                {
                    director.timeUpdateMode = DirectorUpdateMode.Manual;
                }
                director.time = _currentTime;
                director.Evaluate();
            }
        }

        // 슬라이더 탐색 (Scrubbing) 처리
        if (_needsSeek)
        {
            _needsSeek = false;
            
            if (_isPlaying)
            {
                if (director.timeUpdateMode != DirectorUpdateMode.DSPClock)
                {
                    director.timeUpdateMode = DirectorUpdateMode.DSPClock;
                    director.RebuildGraph();
                }
                director.time = _currentTime;
                director.Play();
            }
            else
            {
                if (director.timeUpdateMode != DirectorUpdateMode.Manual)
                {
                    director.timeUpdateMode = DirectorUpdateMode.Manual;
                }
                director.time = _currentTime;
                director.Evaluate();
            }
        }
    }

    void OnDestroy()
    {
        if (_server != null)
        {
            _server.Dispose();
        }
    }
}

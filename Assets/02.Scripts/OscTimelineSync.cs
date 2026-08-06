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
    private float _currentTime = 0f; 
    
    // 슬라이더가 실제로 '움직였는지' 감지하기 위한 변수
    private float _lastReceivedTime = -1f;

    void Start()
    {
        if(director != null) director.timeUpdateMode = DirectorUpdateMode.Manual;

        _server = new OscServer(oscPort);
        _server.MessageDispatcher.AddCallback(addressTime, OnReceiveTime);
        _server.MessageDispatcher.AddCallback(addressPlay, OnReceivePlay);
    }

    // 1. 슬라이더를 움직였을 때
    void OnReceiveTime(string address, OscDataHandle data)
    {
        float incomingTime = data.GetElementAsFloat(0);
        
        // 이전 슬라이더 값과 비교해서, 유저가 실제로 슬라이더를 만져서 값이 변했을 때만 작동!
        if (Mathf.Abs(incomingTime - _lastReceivedTime) > 0.01f)
        {
            _lastReceivedTime = incomingTime;
            
            // 슬라이더가 지시한 시간으로 유니티 시간을 덮어씌움 (Scrubbing)
            _currentTime = incomingTime; 
        }
    }

    // 2. Play/Pause 버튼을 눌렀을 때
    void OnReceivePlay(string address, OscDataHandle data)
    {
        float playValue = data.GetElementAsFloat(0); 
        _isPlaying = (playValue > 0.5f);
    }

    // 3. 매 프레임 업데이트
    void Update()
    {
        if (director == null) return;

        // 재생(Play) 버튼이 켜져 있다면 슬라이더 위치와 상관없이 알아서 굴러감
        if (_isPlaying)
        {
            _currentTime += Time.deltaTime;
        }

        // 타임라인에 시간 적용 및 렌더링
        director.time = _currentTime;
        director.Evaluate(); 
    }

    void OnDestroy()
    {
        if (_server != null) _server.Dispose();
    }
}
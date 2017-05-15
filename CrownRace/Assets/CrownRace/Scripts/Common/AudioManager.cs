using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AudioManager : MonoBehaviour {

	private static AudioManager m_instance;
	private static object m_lockHelper=new object();
	public static AudioManager Instance{
		get{
			if(null == m_instance){
				m_instance = GameObject.FindObjectOfType<AudioManager>();
				if(m_instance == null){
					lock(m_lockHelper){
						if(m_instance == null){
							GameObject go = new GameObject();
							go.name = "AudioManager";
							m_instance = go.AddComponent<AudioManager>();
						}
					}
				}
			}
			return m_instance;
		}
	}
	private List<AudioSource> m_processList = new List<AudioSource>();
	private List<AudioSource> m_catchList = new List<AudioSource>();
	private Dictionary<string, AudioClip> m_clipList = new Dictionary<string, AudioClip>();
	//
	private float volume;
	public float Volume{
		set{
			volume = value;
			ChangeVolume (volume);
		}
		get{ 
			return volume;
		}
	}
	void Awake(){
		DontDestroyOnLoad(this);
	}
	public void ChangeVolume(float volume){
		for (int i = 0; i < m_processList.Count; i++) {
			m_processList [i].volume = volume;
		}
		for (int i = 0; i < m_catchList.Count; i++) {
			m_catchList [i].volume = volume;
		}
		this.volume = volume;
	}
	//
	public void StopAudio(string name){
		int index = -1;
		for(int i=0; i<m_processList.Count; i++){
			if(m_processList[i].clip.name.Equals(name)){
				index = i;
				break;
			}
		}
		if(index >= 0){
			AudioSource src = m_processList[index];
			m_processList.RemoveAt(index);
			src.Stop();
			//src.Pause();
			m_processList.Add(src);
		}
	}
	public void PauseAudio(string name){
		int index = -1;
		for(int i=0; i<m_processList.Count; i++){
			if(m_processList[i].clip.name.Equals(name)){
				index = i;
				break;
			}
		}
		if(index >= 0){
			AudioSource src = m_processList[index];
			src.Pause();
		}
	}
	bool TryGetProcessAudio(string name, out AudioSource src){
		int index = -1;
		for(int i=0; i<m_processList.Count; i++){
			if(m_processList[i].clip.name.Equals(name)){
				index = i;
				break;
			}
		}
		if(index >= 0){
			src = m_processList[index];
			return true;
		}
		src = null;
		return false;
	}
	public void PlayAudio(string name, bool loop=false){
		AudioClip clip;
		AudioSource trySrc;
		if(loop && TryGetProcessAudio(name, out trySrc)){
			if(!trySrc.isPlaying)
				trySrc.Play();
			return;
		}
		if(!m_clipList.TryGetValue(name, out clip)){
			clip = Resources.Load("Audios/"+name, typeof(AudioClip))as AudioClip;
			m_clipList.Add(name, clip);
		}
		AudioSource player;
		if(m_catchList.Count > 0){
			player = m_catchList[0];
			m_catchList.RemoveAt(0);
			m_processList.Add(player);
		}else{
			player = this.gameObject.AddComponent<AudioSource>();
			m_processList.Add(player);
		}
		player.clip = clip;
		player.loop = loop;
		player.minDistance = 0;
		player.volume = volume;
		//player.priority = 0;
		player.transform.position =Camera.main.transform.position;
		player.rolloffMode = AudioRolloffMode.Linear;

		if(!loop){
			player.Play();
			StartCoroutine(OnPlayOver(clip.length, player));
		}else{
			if(player.isPlaying){
				player.UnPause();
			}else{
				player.Play();
			}
		}
	}
	IEnumerator OnPlayOver(float second, AudioSource src){
		yield return new WaitForSeconds(second);
		src.Stop();
		m_processList.Remove(src);
		m_catchList.Add(src);

	}
}

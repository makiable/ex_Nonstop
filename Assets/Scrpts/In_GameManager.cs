using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class In_GameManager : MonoBehaviour {

	//히어로 컨트롤.
	public HeroControl mHero01;

	//몬스터 컨트롤 
	[HideInInspector]
	public List<MonsterControl> mMonster01;


	//텍스트 메세지.
	public TextMesh mIngTextMassage;


	//오토 타겟 몬스터 참조.
	[HideInInspector]
	public MonsterControl TargetMonster;

	private int monsterSpwanNumber = 3;

	// 몬스터 출연 위치.
	public Transform[] mSpwanPoint;
	
	// 던전을 탐험하는 횟수입니다.
	private int mLoopCount = 5;
	
	// 화면에 나타난 적의 합
	private int mMonsterCount = 0;

	// 얼마만큼 뛰다가 적을 만날 것인지.
	private float mRunTime = 1.8f;


	public enum StageStatus
	{
		Idle,
		Start,
		BattleIdle,
		Battle,
		Clear,
	}
	//기본 스테이지 상황.
	public StageStatus mStageStatus = StageStatus.Idle;


	// Use this for initialization
	void Start () {
		// 적 몬스터 들이 담길 List
		mMonster01 = new List<MonsterControl>();
		mMonster01.Clear();
		// 던전 탐험 스텝을 만들어서 순서대로 순환시킵니다.
		StartCoroutine ("AutoStep");

		Debug.Log ("스타트 코루틴");

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	

	IEnumerator AutoStep()
	{
		while (true) 
		{
			if (mStageStatus == StageStatus.Idle) 
			{

				//스타트 TEXT 출현..
				mIngTextMassage.text = "스타트!";

				yield return new WaitForSeconds (1.2f);

				mStageStatus = StageStatus.BattleIdle;

			}
			else if (mStageStatus == StageStatus.BattleIdle) {
				//몬스터 등장...
				mHero01.SetStatus(HeroControl.Status.Idle);
				mMonster01.Clear();
				for (int i = 0; i < monsterSpwanNumber; i++) {
					//X 마리의 몬스터를 소환 합니다.
					SpawnMonster(i);

					//딜레이를 둔다. for 문에 딜레이를 줌.
					yield return new WaitForSeconds(0.12f);
				}

				yield return new WaitForSeconds(2); // 2초 대기..

				mIngTextMassage.text="*배틀 스타트*";

				//배틀 상태로 둔다..
				mStageStatus = StageStatus.Battle;

				//코루틴 실행.
				StartCoroutine("HeroAutoAttack");
				//StartCoroutine("MonsterAutoAttack");
				yield break;
			}
		}
	}
	

	private void SpawnMonster(int idx)
	{
		Debug.Log ("몬스터 출현 시작");
		// Resources 폴더로부터 Monster 프리팹(Prefab)을 로드합니다.
		Object prefab = Resources.Load("Monster01");

		Debug.Log ("리소스 로드 완료");

		// 참조한 프리팹을 인스턴스화 합니다. (화면에 나타납니다.)
		GameObject monster = Instantiate(prefab, mSpwanPoint[idx].position, Quaternion.identity) as GameObject;

		//위치 값이 이상해서, 수동으로 조절 했음.
		monster.transform.parent = mSpwanPoint[idx];


		Debug.Log ("idx="+idx);
		
		
		// 생성된 인스턴스에서 MonsterControl 컴포넌트를 불러내어 mMonster 리스트에 Add 시킵니다.
		mMonster01.Add(monster.GetComponent<MonsterControl>());
		
		
		// 생성된 몬스터 만큼 카운팅 됩니다.
		mMonsterCount += 1;
		
		mMonster01[idx].idx = idx;
		mMonster01[idx].RandomHP();//

		Debug.Log ("monster Hp = " + mMonster01[idx].mHP);
		monster.name = "Monster01"+idx;
		// 레이어 오더를 단계적으로 주어 몬스터들의 뎁스가 차례대로 겹치도록 한다.
		monster.GetComponent<SpriteRenderer>().sortingOrder = idx+1;

		Debug.Log ("완료");

	}

	IEnumerator HeroAutoAttack(){

		GetSingleAutoTarget ();

		while (mStageStatus == StageStatus.Battle) {
			//공격 애니메이션 추가.
			mHero01.SetStatus(HeroControl.Status.Attack);
			//한번 공격하고 공격 속도 만큼 멈춘다.
			yield return new WaitForSeconds(mHero01.mAttackSpeed);

		}
	}

	//IEnumerator HeroAutoAttack01(){

	//	GetSingleAutoTarget ();

	//	while (mStageStatus == StageStatus.Battle) {
	//		Invoke("heroMontionAttack", 2f);
	//	}
	//}
	//void heroMontionAttack(){
	//	mHero01.SetStatus (HeroControl.Status.Attack);
	//}


	private void GetSingleAutoTarget(){

		//포지션에서 맨 앞에 있는 애를 잡도록 나중에 수정이 필요함.. 지금은 HP분으로 체크..
		TargetMonster = mMonster01.Where(m=>m.mHP > 0).OrderBy(m=>m.mHP).First();

		TargetMonster.SetSingleTarget ();


	}






























	// 버튼의 명령을 받았을때, 처리 하는 곳.
	void OnButtonDown(string trigger){
		if (trigger == "Back") {
			//mCameraControl.SetStatus(CameraControl.Status.Start);
			Invoke("StartButton",0.5f);
		}


		//캐릭터 상태 동작.
		if (trigger == "ChangeStatus") {
			mHero01.SetStatus(HeroControl.Status.Attack);
			Debug.Log("Status Change good");

			//mMonster01.SetStatus(MonsterControl.Status.Damaged);
		}

		if (trigger == "StopStatus") {
			mHero01.SetStatus(HeroControl.Status.Idle);
			Debug.Log("idle ok");

			//mMonster01.SetStatus(MonsterControl.Status.UseSkill);

		}
		if (trigger == "UseSkill") {
			mHero01.SetStatus(HeroControl.Status.UseSkill);
			Debug.Log("UseSkill Change good");

			//mMonster01.SetStatus(MonsterControl.Status.Dead);
		}
		
		if (trigger == "Damaged") {
			mHero01.SetStatus(HeroControl.Status.Damaged);
			Debug.Log("Damaged ok");

			//mMonster01.SetStatus(MonsterControl.Status.Attack);
		}
		if (trigger == "Dead") {
			mHero01.SetStatus(HeroControl.Status.Dead);
			Debug.Log("Dead ok");

			//mMonster01.SetStatus(MonsterControl.Status.Idle);

		}
		
	}
	// 버튼의 명령 처리 함수 
	void StartButton(){
		Application.LoadLevel("Menu_Default_Scene");
	}

}

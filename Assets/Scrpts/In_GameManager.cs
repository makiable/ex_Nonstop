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
	

	//오토 타겟 몬스터 참조. 1명을 잡을때
	[HideInInspector]
	public MonsterControl TargetMonster;

	public MonsterControl[] AllTargetMonster;

	private int monsterSpwanNumber = 3;

	// 몬스터 출연 위치.
	public Transform[] mSpwanPoint;
	
	// 던전을 탐험하는 횟수입니다.
	private int mLoopCount = 2;
	
	// 화면에 나타난 적의 합
	private int mMonsterCount = 0;

	// 얼마만큼 뛰다가 적을 만날 것인지.
	//private float mRunTime = 1.8f;


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
				StartCoroutine("HeroSkillAttack");
				StartCoroutine("MonsterAutoAttack");
				yield break;
			}
		}
	}
	

	private void SpawnMonster(int idx)
	{
		// Resources 폴더로부터 Monster 프리팹(Prefab)을 로드합니다.
		Object prefab = Resources.Load("Monster01");

		// 참조한 프리팹을 인스턴스화 합니다. (화면에 나타납니다.)
		GameObject monster = Instantiate(prefab, mSpwanPoint[idx].position, Quaternion.identity) as GameObject;

		//위치 값이 이상해서, 수동으로 조절 했음.
		monster.transform.parent = mSpwanPoint[idx];
		
		
		// 생성된 인스턴스에서 MonsterControl 컴포넌트를 불러내어 mMonster 리스트에 Add 시킵니다.
		mMonster01.Add(monster.GetComponent<MonsterControl>());
		
		
		// 생성된 몬스터 만큼 카운팅 됩니다.
		mMonsterCount += 1;
		
		mMonster01[idx].idx = idx;
		mMonster01[idx].RandomHP();//
		mMonster01 [idx].monsterHPtext.text = mMonster01 [idx].mHP.ToString ();

		mMonster01 [idx].TargetNumber = idx+1;
		monster.name = "Monster01"+idx;
		// 레이어 오더를 단계적으로 주어 몬스터들의 뎁스가 차례대로 겹치도록 한다.
		monster.GetComponent<SpriteRenderer>().sortingOrder = idx+1;


	}

	IEnumerator HeroAutoAttack(){

		//타겟을 잡고..
		GetSingleAutoTarget ();

		while (mStageStatus == StageStatus.Battle) {
			//공격 애니메이션 추가.
			mHero01.SetStatus(HeroControl.Status.Attack);

			//실제로 데미지도 들어가야함. (애니메이션은 내용에 포함)
			TargetMonster.Damaged();

			//mIngTextMassage.text = "적에게 데미지:"+ TargetMonster.saveDamageTextForShow+"을 입혔다.";

			//한번 공격하고 공격 속도 만큼 멈춘다.
			yield return new WaitForSeconds(mHero01.mAttackSpeed);
		}
	}

	IEnumerator HeroSkillAttack(){
		GetAllAutoTarget ();
		while (mStageStatus == StageStatus.Battle) {
			yield return new WaitForSeconds(mHero01.mSkillSpeed);
			mHero01.SetStatus(HeroControl.Status.UseSkill);

			if (mMonster01[0] != null) {
				mMonster01[0].Damaged();
				Debug.Log("몬스터 0에게 데미지 ="+mMonster01[0].saveDamageTextForShow);
			}

			if (mMonster01[1] != null) {
				mMonster01[1].Damaged();
				Debug.Log("몬스터 1에게 데미지 ="+mMonster01[1].saveDamageTextForShow);
			}

			if (mMonster01[2] != null) {
				mMonster01[2].Damaged();
				Debug.Log("몬스터 1에게 데미지 ="+mMonster01[2].saveDamageTextForShow);
			}

			//이걸 쓰면 몬스터에게 광역이 안들어감 하나를 못찾음..
			//for (int i = 0; i < mMonsterCount; i++) {
			//	if (mMonster01[i] != null) {
			//		mMonster01[i].Damaged();
			//		Debug.Log("몬스터 "+i+"에게 "+mMonster01[i].saveDamageTextForShow+"를 주었다. 남은 HP = "+mMonster01[i].mHP);
			//	}
			//Debug.Log("몬스터0 hp= "+mMonster01[0].mHP+"..몬스터 1 hp = "+mMonster01[1].mHP+"..몬스터 2 hp ="+mMonster01[2].mHP);
			//mMonster01.Damaged();
		}
	}

	private void GetAllAutoTarget(){
	//	TargetMonster.SetAllTarget ();
	}
	
	
	private void GetSingleAutoTarget(){
		//1. HP로 소팅할 경우 이거 참조..
		//TargetMonster = mMonster01.Where(m=>m.mHP > 0).OrderBy(m=>m.mHP).First();
		//2. 타겟 넘버를 지정 맨 앞에 를 타겟으로 잡는다.
		TargetMonster = mMonster01.Where (m => m.TargetNumber > 0).OrderBy (m => m.TargetNumber).First ();
		TargetMonster.SetSingleTarget ();
	}

	public void ReAutoTarget(){
		mMonsterCount -= 1;
		Debug.Log ("몬스터 카운트= "+mMonsterCount);

		TargetMonster = null;

		if (mMonsterCount == 0) {
			//한 스테이지 클리어 
			mLoopCount -= 1;

			StopCoroutine ("HeroAutoAttack");
			StopCoroutine ("HeroSkillAttack");

			//mIngTextMassage.text = "모든 적을 격파";

			if (mLoopCount == 0) {
				//모든 스테이지 클리어. -> 승리 결과창.

				mIngTextMassage.text = "스테이지 클리어!";

				//Invoke ("GameOver", 2);
				GameOver();
				return;
			}

			mStageStatus = StageStatus.Idle;
			StartCoroutine ("AutoStep");
			return;
		}
		//타겟 재 탐색.
		GetSingleAutoTarget();
	}

	// 몬스터 자동 공격 입니다잉~~~
	IEnumerator MonsterAutoAttack(){
		//타겟을 찾습니다..
		GetMonsterSingleAutoTarget ();

		while (mStageStatus == StageStatus.Battle) {
			//공격에 들어갑니다. 몬스터는 여러마리..

			foreach (MonsterControl monster in mMonster01) { //모든 몬스터를 하나씩 돌린다..
				if (monster.mStatus == MonsterControl.Status.Dead) continue;
				monster.mAnimator.SetTrigger("Attack");

				mHero01.heroAttackedMonsterNormal(monster.mAttack);

				yield return new WaitForSeconds(monster.mAttackSpeed + Random.Range(0, 0.5f));
			}
		}
	}

	

	private void GetMonsterSingleAutoTarget(){

		//지금은 한명이니, 1인에가 타겟임..
		mHero01.mHeroSingleTargeted = true;
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

			//mMonster01.SetStatus(MonsterControl.Status.Damaged);
		}

		if (trigger == "StopStatus") {
			mHero01.SetStatus(HeroControl.Status.Idle);

			//mMonster01.SetStatus(MonsterControl.Status.UseSkill);

		}
		if (trigger == "UseSkill") {
			mHero01.SetStatus(HeroControl.Status.UseSkill);

			//mMonster01.SetStatus(MonsterControl.Status.Dead);
		}
		
		if (trigger == "Damaged") {
			mHero01.SetStatus(HeroControl.Status.Damaged);

			//mMonster01.SetStatus(MonsterControl.Status.Attack);
		}
		if (trigger == "Dead") {
			mHero01.SetStatus(HeroControl.Status.Dead);

			//mMonster01.SetStatus(MonsterControl.Status.Idle);

		}
		
	}
	// 버튼의 명령 처리 함수 
	void StartButton(){
		Application.LoadLevel("Menu_Default_Scene");
	}

	public void GameOver(){
		//임시
		Debug.Log("GameOver");    
		StopCoroutine ("HeroAutoAttack");
		StopCoroutine ("HeroSkillAttack");
		StopCoroutine ("MonsterAutoAttack");
		StopCoroutine ("AutoStep");
		//Application.LoadLevel("Menu_Default_Scene");

	}


}

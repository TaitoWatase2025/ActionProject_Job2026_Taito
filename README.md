# Unity 個人制作「ソウルライク風バトルゲーム」
# 🚫 Job Application Portfolio Only
このリポジトリは就活用ポートフォリオです。
指導講師：小西秀明（職業訓練校 Unity講座）

---

## 目次
- [プロジェクト概要](#project-overview)
- [使用予定のUnity機能](#unity-features)
- [進行スケジュール（メモ）](#schedule)
- [発表内容（予定）](#presentation-plan)
- [工夫した点](#Technical)


## プロジェクト概要 <a id="project-overview"></a>

| 項目 | 内容 |
|------|------|
| **ゲームタイトル** | Refleam |
| **一言で説明すると** | 敵の動きを見て倒すアクションゲーム |
| **ゲームの目的** | 敵を倒す |
| **プレイヤー操作** | WASD操作、クリック攻撃・・・ |
| **勝利条件** | 敵をすべて倒すこと |
| **失敗条件** | 自分が倒されること |
| **特徴・工夫点** | 演出面と難易度 |

---

## 使用予定のUnity機能 <a id="unity-features"></a>

- [x] PlayerController（移動・入力）  
- [x] Trigger / Collider  
- [x] UI（Canvas, Button, TextMeshPro）  
- [ ] ScriptableObject / JSONデータ  
- [x] アニメーション / DOTween  
- [x] サウンド（BGM・SE）  
- [x] NavMesh / AI  
- [x] Timeline / Cinemachine  
- [ ] その他（　　　　　　　　　　　　　　　　　　　　）

---

## 進行スケジュールメモ <a id="schedule"></a>

| 日 | 内容 | 主な成果物 |
|----|------|------------|
| 1日目 | 企画書作成・Git初期化 | README.md作成 |
| 2日目 | Mainキャラクター作成 |　主要スクリプト作成  |
| 3日目 | 操作関連実装 | 操作移動・ジャンプ・スプリント・カメラ操作作成 |
| 4日目 | アニメーション実装 | 各操作に合わせたAnimator作成 |
| 5日目 | 修正処理・操作追加 | 操作とアニメを一括管理・攻撃作成 |
| 6日目 | 修正・操作追加 | 回避・コンボ攻撃・モーション修正 |
| 7日目 | 修正 | コンボ攻撃修正 |
| 8日目 | 敵キャラクターを作成 | 主要スクリプト作成 |
| 9日目 | 修正 | 敵挙動を修正 |
| 10日目 | アニメーション関連 | レイヤーマスク実装 |
| 11日目 | 敵AI面実装・修正 | 距離に合わせて回避行動・レイヤー削除 |
| 12日目 | 敵AI面実装・修正 | 近距離行動・攻撃の有効化 |
| 13日目 | 戦闘関連作成 | ヒットストップ作成・ガード実装 |
| 14日目 | 戦闘演出関連作成 | Area攻撃実装・パーティクル実装 |
| 15日目 | 演出関連作成 | タイトル画面作成 |
| 16日目 | 演出関連作成 | タイトルシーン実装 |
| 17日目 | 戦闘関連の修正 | 敵キャラの挙動の不具合を修正・Push攻撃実装 |
| 18日目 | 演出関連作成 | ステージの作成・Game画面のUI実装 |
| 19日目 | 演出関連作成 | ステージ実装・敵スポナーを実装 |
| 20日目 | 演出関連修正 | ステージを鏡面化 |
| 21日目 | 不具合修正 | オブジェクト挙動修正 |
| 22日目 | クリア要素実装 | Prefab追加、クリアシーン作成、左記に伴う演出実装 |
| 23日目 | 演出関連実装  | ゲーム内案内の実装、Textscene作成 |
| 24日目 | 発表 | 成果物の発表と仕様紹介 |
 

- **フォルダ構成推奨**
```
Assets/
├─ Scripts/
├─ Prefabs/
├─ Scenes/
├─ UI/
└─ Audio/
```
---

## 発表内容（予定） <a id="presentation-plan"></a>

| 項目 | 内容 |
|------|------|
| **タイトル** | Refleam |
| **ゲーム紹介** | うまく立ち回り敵を倒しまくれ！ |
| **アピールポイント** | 敵AIの挙動に注力 |
| **頑張った点** | 敵AIの挙動が思うように動かず、修正に時間がかかった。 |

---
## 工夫した点 <a id="Technical"></a>
> 
>
> 
高度な敵AIの実装
このプロジェクトでは、プレイヤーに戦略性とやりがいのある戦闘体験を提供するため、以下の点で高度なAIロジックを設計し、実装しました。

1. 複雑な状況判断とランダム性の両立による戦闘の多様化

➡️ 課題と目的
単調な行動パターンによる飽きや、パターン化による攻略の容易さを防ぎ、プレイヤーに常に新しい対応を求める戦闘システムを構築すること。

➡️ 工夫した点
距離・HPによる多層的な行動決定ロジックの設計
プレイヤーとの距離を主要な判断基準とし、近距離、中距離、それ以上で実行可能な行動群を分けました。
さらに、敵のHPが80%以下になった場合、「スタン攻撃」が行動パターンに追加される制御ロジックに組み込み、戦闘の緊張感を高めました。
「確定制御」と「ランダム性」のバランス調整
Random.Valueを用いた確率判定を導入することで、AIに一定の予測不能性を持たせつつ、特定行動が連続しないように**LastActionを記録・監視**し、連続行動を制御するロジックを実装しました。これにより、自然かつ多様な行動パターンを実現しました。

<details>
<summary><b>コアロジック（C#）を表示 🔽</b></summary>
// 敵AIの行動決定ロジック（抜粋）
 
    // HPが50%以下 かつ 近距離 の場合に「AreaAttack」を確率で実行
    if ((float)Status.health / Status.maxHealth <= 0.5f && // 1. HP条件（フェーズ移行）
        distance < shortRange &&
        Random.value < 0.5f &&
        lastAction != LastAction.AreaAttack) // 3. 連続行動防止
    {
        // 速度ランダム化
        anim.speed = Random.Range(0.5f, 2f); 
        anim.SetTrigger("AreaAttack");
        lastAction = LastAction.AreaAttack;
        isAttacking = true;
        return; // 確定攻撃のため即座に終了
    }

    // 近距離（minRange未満）での行動群
    if (distance < minRange)
    {
        // バックステップ（確率かつクールタイム管理）
        if (Random.value < 0.5f && Time.time - lastAttackTime >= attackCooldown)
        {
            anim.SetTrigger("BackJump");
            lastAction = LastAction.BackJump;
            // ...
            return;
        }
        // 通常近距離攻撃（正面角度、連続行動防止）
        else if (lastAction != LastAction.ShortAttack && angle < attackAngle && Time.time - lastAttackTime >= attackCooldown)
        {
            anim.SetTrigger("ShortAttack");
            lastAction = LastAction.ShortAttack;
            // ...
            return;
        }
        // ... 他の近距離行動 ...
    }

    // 中距離での行動群
    if (distance < attackRange && distance > minRange && 
        angle < attackAngle && Time.time - lastAttackTime >= attackCooldown &&
        lastAction != LastAction.Attack)
    {
        anim.SetTrigger("Attack");
        lastAction = LastAction.Attack;
        // ...
    }

    // ... 後略 ...
}
***
【コードの解説】

Status.health / Status.maxHealth <= 0.5f の条件により、敵のHPが50%を切った際にのみ、AreaAttack が選択肢に入ります。（フェーズ移行制御）

lastAction != LastAction.AreaAttack で、同一行動の連続発動を防いでいます。（予測不能性の確保）

if (distance < minRange) や if (distance < attackRange && distance > minRange) で、距離に応じた適切な行動群への分岐を明確にしています。（行動階層化の実現）
***
</details>

2. プレイヤーの位置と入力への動的な対応

➡️ 課題と目的
プレイヤーの動きに対して単に追いかけるだけでなく、意図的に特定の回避行動を強いる、リアリティのあるAI動作を実現すること。

➡️ 工夫した点
プレイヤー位置に基づく精密な行動判断
プレイヤーの現在位置を取得し、**「敵の正面角度」**を動的に計算することで、プレイヤーが背後に回り込んだ際の攻撃のキャンセルや、回避行動（バックステップなど）の実行を正確に制御しました。

<details>
<summary><b>コアロジック（C#）を表示 🔽</b></summary>

    // ... 追跡ロジック（速度変化など） ...
    if (distance <= ShortRange) // 近い距離にいる場合のみ回転制御を有効化
    {
     // プレイヤー方向を計算
     Vector3 direction = (player.position - transform.position).normalized;
     // 正面との角度を計算
     float angle = Vector3.Angle(transform.forward, direction);
        
      if (angle > 5f) // 一定以上の角度ズレがある場合、回転を開始
      {
          agent.isStopped = true; // 回転中は移動を停止
        
          // どちらの方向に回転すべきかを判定 (Y軸の符号で方向を決定)
          Vector3 cross = Vector3.Cross(transform.forward, direction);
          float sign = Mathf.Sign(cross.y); 

          float rotationSpeed = 180f; // 回転速度
          transform.Rotate(0, sign * rotationSpeed * Time.deltaTime, 0);
      }
    }
***
敵AIは、単にプレイヤーに向かって移動するだけでなく、近距離戦においてプレイヤーの背後への回り込みを防ぐため、常に正面角度を正確に維持するロジックを実装しています。

以下のコードでは、プレイヤーとの角度が5度以上ずれた場合、NavMeshAgentを一時停止し、**手動でプレイヤー方向へ回転（方向ベクトルと外積計算を使用）**させることで、攻撃タイミングでのズレや不自然な挙動を最小限に抑えています。
***
</details>

4. プレイヤーの回避行動を制限する攻撃設計

➡️ 課題と目的
単なる攻撃の回避ではなく、**「適切なタイミングで、適切な回避アクションを選択させる」**というゲームプレイの深みを生み出すこと。

➡️ 工夫した点
特定の回避アクションのみ有効な「強制的な攻撃パターン」の設計
敵の主要な特殊攻撃である「スタン攻撃」と「吹き飛ばし攻撃」に対し、それぞれ**「ジャンプ」と「Dodge（緊急回避）」という特定の回避アクション**のみを成功判定としました。
これにより、プレイヤーは敵の攻撃アニメーションを見て、瞬時に必要な回避アクションを判断し実行する必要があり、戦闘の難易度と没入感を高めることに成功しました。
<details>
<summary><b>コアロジック（C#）を表示 🔽</b></summary>
 
     public void OnPushAttackHit(Transform enemyTransform)
     {
      if(isDodging) return; // 🌟 Dodge中は無効化 🌟
      // ... 続く被弾・スタン処理 ...
      StartPush(enemyTransform); 
     }
敵の「吹き飛ばし攻撃」を受けた際に、プレイヤーが**isDodging（緊急回避中）**であれば、直ちに被弾処理をキャンセルし、吹き飛ばしを無効化していることを示します。

スタン攻撃の判定領域（ヒットボックス）として、地面に近い位置（足元など）にコライダーを設定している。これにより、地上の敵にのみ影響を与えたり、特定の高さにいる敵（ジャンプ中の敵）には当たらないようにしている。

</details>

---

© 2025 職業訓練校 Unity講座（講師：小西秀明）

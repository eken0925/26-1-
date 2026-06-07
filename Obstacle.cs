using UnityEngine;

// 장애물 오브젝트에 붙이는 스크립트
// 플레이어가 이 오브젝트의 트리거 범위에 닿으면
// 플레이어를 시작 위치로 리스폰시킨다.
public class Obstacle : MonoBehaviour
{
    // 다른 Collider2D가 이 오브젝트의 Trigger 영역에 들어왔을 때 호출됨
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 들어온 오브젝트의 태그가 "Player"인지 확인
        // 플레이어가 아니면 아무 작업도 하지 않음
        if (other.CompareTag("Player"))
        {
            // 충돌한 오브젝트에서 PlayerGravityController2D 컴포넌트를 가져옴
            // 이 스크립트 안에 RespawnToStart() 메서드가 들어 있음
            PlayerGravityController2D player = other.GetComponent<PlayerGravityController2D>();

            // PlayerGravityController2D가 실제로 붙어 있는 경우에만 실행
            // 혹시 플레이어 태그는 있지만 해당 스크립트가 없을 수도 있으므로 null 체크
            if (player != null)
            {
                // 플레이어를 시작 위치(startPoint)로 되돌리는 리스폰 함수 실행
                player.RespawnToStart();
            }
        }
    }
}

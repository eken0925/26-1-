using UnityEngine;
using UnityEngine.SceneManagement;

// 씬 전환용 오브젝트(예: 포탈, 문, 출구)에 붙이는 스크립트
// 플레이어가 범위 안에 들어온 상태에서 위쪽 화살표 키를 누르면
// 지정한 다음 씬으로 이동한다.
public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "Stage01";
    // 이동할 다음 씬 이름
    // 반드시 Build Settings에 등록된 씬 이름과 정확히 같아야 함

    private bool playerInRange = false;
    // 플레이어가 현재 이 오브젝트의 트리거 범위 안에 있는지 저장

    private void Update()
    {
        // 플레이어가 범위 안에 있고,
        // 위쪽 화살표 키를 누른 순간에만 씬 이동 실행
        if (playerInRange && Input.GetKeyDown(KeyCode.UpArrow))
        {
            // nextSceneName에 적힌 이름의 씬을 불러옴
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 트리거 범위 안에 들어온 오브젝트가 Player 태그인지 확인
        if (other.CompareTag("Player"))
        {
            // 플레이어가 범위 안에 들어왔으므로 씬 이동 가능 상태로 변경
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 트리거 범위를 벗어난 오브젝트가 Player 태그인지 확인
        if (other.CompareTag("Player"))
        {
            // 플레이어가 범위를 벗어났으므로 씬 이동 불가 상태로 변경
            playerInRange = false;
        }
    }
}

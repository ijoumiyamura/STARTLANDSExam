using UnityEngine;

public class Billboard : MonoBehaviour
{
    private void Update() {
        transform.rotation = Camera.main.transform.rotation;
    }
}

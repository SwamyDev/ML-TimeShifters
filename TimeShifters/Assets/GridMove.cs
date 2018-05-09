using UnityEngine;

public class GridMove : MonoBehaviour {
    public int StepSize;

    public void Up() {
        OffsetYPosition(StepSize);
    }

    private void OffsetYPosition(int offset) {
        var p = transform.position;
        p.y += offset;
        transform.position = p;
    }

    public void Down() {
        OffsetYPosition(-StepSize);
    }

    public void Right() {
        OffsetXPosition(StepSize);
    }

    private void OffsetXPosition(int offset) {
        var p = transform.position;
        p.x += offset;
        transform.position = p;
    }

    public void Left() {
        OffsetXPosition(-StepSize);
    }
}
using NUnit.Framework;
using UnityEngine;

namespace UnitTests {

public class GridMoveTests {

	[Test]
	public void MoveUp() {
		var move = MakeGridMove(stepSize: 10);
		move.Up();
		move.Up();
		Assert.That(move.transform.position.y, Is.EqualTo(20));
	}

	private GridMove MakeGridMove(int stepSize) {
		var m = new GameObject("GridMove").AddComponent<GridMove>();
		m.StepSize = stepSize;
		return m;
	}

	[Test]
	public void MoveDown() {
		var move = MakeGridMove(stepSize: 10);
		move.Down();
		move.Down();
		Assert.That(move.transform.position.y, Is.EqualTo(-20));
	}

	[Test]
	public void MoveRight() {
		var move = MakeGridMove(stepSize: 10);
		move.Right();
		move.Right();
		Assert.That(move.transform.position.x, Is.EqualTo(20));
	}

	[Test]
	public void MoveLeft() {
		var move = MakeGridMove(stepSize: 10);
		move.Left();
		move.Left();
		Assert.That(move.transform.position.x, Is.EqualTo(-20));
	}
}

}
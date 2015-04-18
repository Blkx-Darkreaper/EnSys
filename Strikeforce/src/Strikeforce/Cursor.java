package Strikeforce;

import java.awt.Rectangle;
import java.awt.event.KeyEvent;
import static Strikeforce.Global.*;
import static Strikeforce.Board.*;

public class Cursor extends Entity {
	private final int moveUpKey = KeyEvent.VK_UP;
	private final int moveDownKey = KeyEvent.VK_DOWN;
	private final int moveLeftKey = KeyEvent.VK_LEFT;
	private final int moveRightKey = KeyEvent.VK_RIGHT;
	private final int selectKey = KeyEvent.VK_A;
	private final int modifyKey = KeyEvent.VK_S;
	private final int buildMenuKey = KeyEvent.VK_D;
	private Building selectedBuilding;

	public Cursor(String inName, int inX, int inY) {
		super(inName, inX, inY, 0, 0);
	}

	public void shiftLeft() {
		centerX -= CELL_SIZE;
		int leftEdge = CELL_SIZE / 2;
		if (centerX < leftEdge) {
			centerX = leftEdge;
		}
		if (selectedBuilding == null) {
			return;
		}
		selectedBuilding.shiftLeft();
	}

	public void shiftRight() {
		centerX += CELL_SIZE;
		int rightEdge = LEVEL_WIDTH - CELL_SIZE / 2;
		if (centerX > rightEdge) {
			centerX = rightEdge;
		}
		if (selectedBuilding == null) {
			return;
		}
		selectedBuilding.shiftRight();
	}

	public void shiftUp() {
		centerY += CELL_SIZE;
		int topEdge = levelTop - CELL_SIZE / 2;
		if (centerY > topEdge) {
			centerY = topEdge;
		}
		if (selectedBuilding == null) {
			return;
		}
		selectedBuilding.shiftUp();
	}

	public void shiftDown() {
		centerY -= CELL_SIZE;
		int bottomEdge = CELL_SIZE / 2;
		if (centerY < bottomEdge) {
			centerY = bottomEdge;
		}
		if (selectedBuilding == null) {
			return;
		}
		selectedBuilding.shiftDown();
	}

	public void select() {
		Rectangle cursorBox = getBounds();
		selectedBuilding = selectBuildingInArea(cursorBox, null);
		if (selectedBuilding == null) {
			return;
		}
		selectedBuilding.setIsSelected(true);
	}

	public void deselect() {
		if (selectedBuilding == null) {
			return;
		}
		Rectangle area = selectedBuilding.getBounds();
		// boolean areaOccupied = checkAreaForBuildings(area);
		Building obstruction = selectBuildingInArea(area, selectedBuilding);
		if (obstruction != null) {
			illegalAction();
			obstruction.setIsSelected(true);
			return;
		}
		selectedBuilding.setIsSelected(false);
		selectedBuilding = null;
	}

	private void illegalAction() {
		return;
	}

	public void update() {
		return;
	}

	public void keyPressed(KeyEvent e) {
		int key = e.getKeyCode();
		switch (key) {
		case moveLeftKey:
			shiftLeft();
			break;
		case moveRightKey:
			shiftRight();
			break;
		case moveUpKey:
			shiftUp();
			break;
		case moveDownKey:
			shiftDown();
			break;
		case selectKey:
			if (selectedBuilding != null) {
				deselect();
				break;
			}
			select();
			break;
		case modifyKey:
			if (selectedBuilding == null) {
				break;
			}
			selectedBuilding.rotate();
			break;
		case buildMenuKey:
		}
	}

	public void keyReleased(KeyEvent e) {
		int key = e.getKeyCode();
		switch (key) {
		default:
			break;
		}
	}
}
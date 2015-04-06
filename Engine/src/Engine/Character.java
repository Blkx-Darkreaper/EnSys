package Engine;
import static Engine.Global.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.KeyEvent;
import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import javax.swing.Timer;

public class Character implements ActionListener {
	
	protected Timer time;
	protected Behaviour slave;
	protected Map<Integer, Method> keyBindings = new HashMap<>();
	protected enum Direction {Up, Down, Left, Right, Nothing};
	protected Direction vertDirection = Direction.Nothing;
	protected Direction horDirection = Direction.Nothing;
	protected enum TurnDirection {Left, Right, Nothing};
	protected TurnDirection turn = TurnDirection.Nothing;
	protected enum Acceleration {Accelerate, Decelerate, Nothing};
	protected Acceleration acceleration = Acceleration.Nothing;
	
	public Character() {
		time = new Timer(DEFAULT_TIME_INTERVAL, this);
		time.start();
	}
	
	public Behaviour getBehaviour() {
		return slave;
	}
	
	public void setBehaviour(Behaviour inBehaviour) {
		slave = inBehaviour;
	}
	
	public void setEntity(Entity inEntity) {
		if(slave == null) {
			throw new IllegalStateException("Behaviour must be set first");
		}
		
		slave.setEntity(inEntity);
	}
	
	public Method getKeyAction(int key, int eventType) {
		int keyCode = key * eventType;
		Method toPerform = keyBindings.get(keyCode);
		
		return toPerform;
	}
	
	public void addKeyBinding(char key, boolean pressed, String methodName) {
		int keyCode = KeyEvent.getExtendedKeyCodeForChar(key);
		int eventType = KeyEvent.KEY_PRESSED;
		if(pressed == false) {
			eventType = KeyEvent.KEY_RELEASED;
		}
		
		Method method;
		
		try {
			Class[] params = {};
			Class className = this.getClass();
			method = className.getMethod(methodName, params);
		} catch (NoSuchMethodException e) {
			e.printStackTrace();
			return;
		} catch (SecurityException e) {
			e.printStackTrace();
			return;
		}
		
		//String keyCode = String.valueOf(key);
		keyCode = keyCode * eventType;
		keyBindings.put(keyCode, method);
	}

	@Override
	public void actionPerformed(ActionEvent arg0) {
		switch (vertDirection) {
		case Up:
			slave.moveUp();
			break;
		case Down:
			slave.moveDown();
			break;
		default:
			break;
		}
		
		switch (horDirection) {
		case Right:
			slave.moveRight();
			break;
		case Left:
			slave.moveLeft();
			break;
		default:
			break;
		}
		
		switch (turn) {
		case Left:
			slave.turnRight();
			break;
		case Right:
			slave.turnLeft();
			break;
		default:
			break;
		}
		
		switch (acceleration) {
		case Accelerate:
			slave.accelerate();
			break;
		case Decelerate:
			slave.decelerate();
			break;
		default:
			break;
		}
	}

	public void startMovingUp() {
		vertDirection = Direction.Up;
	}
	
	public void stopMovingUp() {
		if(vertDirection != Direction.Up) {
			return;
		}
		
		vertDirection = Direction.Nothing;
	}
	
	public void startMovingDown() {

		vertDirection = Direction.Down;
	}
	
	public void stopMovingDown() {
		if(vertDirection != Direction.Down) {
			return;
		}
		
		vertDirection = Direction.Nothing;
	}
	
	public void startMovingRight() {
		horDirection = Direction.Right;
	}
	
	public void stopMovingRight() {
		if(horDirection != Direction.Right) {
			return;
		}
		
		horDirection = Direction.Nothing;
	}
	
	public void startMovingLeft() {
		horDirection = Direction.Left;
	}
	
	public void stopMovingLeft() {
		if(horDirection != Direction.Left) {
			return;
		}
		
		horDirection = Direction.Nothing;
	}
	
	public void startTurningLeft() {
		turn = TurnDirection.Left;
	}
	
	public void stopTurningLeft() {
		if(turn != TurnDirection.Left) {
			return;
		}
		
		turn = TurnDirection.Nothing;
	}
	
	public void startTurningRight() {
		turn = TurnDirection.Right;
	}
	
	public void stopTurningRight() {
		if(turn != TurnDirection.Right) {
			return;
		}
		
		turn = TurnDirection.Nothing;
	}
	
	public void startAccelerating() {
		acceleration = Acceleration.Accelerate;
	}
	
	public void stopAccelerating() {
		if(acceleration != Acceleration.Accelerate) {
			return;
		}
		
		acceleration = Acceleration.Nothing;
	}
	
	public void startDecelerating() {
		acceleration = Acceleration.Decelerate;
	}
	
	public void stopDecelerating() {
		if(acceleration != Acceleration.Decelerate) {
			return;
		}
		
		acceleration = Acceleration.Nothing;
	}

    public void headToWaypoint() {}
    
    private int distanceToPoint(int pointX, int pointY) {
    	int centerX = slave.getEntity().getCenterX();
    	int centerY = slave.getEntity().getCenterY();
    	
		int distanceX = centerX - pointX;
		int distanceY = centerY - pointY;
		
		int distanceToPoint = (int) Math.sqrt(distanceX * distanceX + distanceY * distanceY);
		return distanceToPoint;
    }
    
    private int headingToPoint(int pointX, int pointY) {
    	int centerX = slave.getEntity().getCenterX();
    	int centerY = slave.getEntity().getCenterY();
    	
    	int distanceX = pointX - centerX;
		int distanceY = pointY - centerY;
		
		int heading = 360;
		
		if(distanceY == 0) {
			if(distanceX < 0) {
				heading = 270;
				return heading;
			}
			
			heading = 90;
			return heading;
		}
		
		if(distanceY < 0) {
			heading = 180;
		}
		
		double value = distanceX;
		double value2 = distanceY;
		
		heading += (int) Math.toDegrees(Math.atan(value / value2));
		heading %= 360;
		
		return heading;
    }
    
    private void moveToPoint(int pointX, int pointY, int movementSpeed) {
    	int speed = slave.getEntity().getSpeed();
    	
    	int distanceToPoint = distanceToPoint(pointX, pointY);
		
		if(distanceToPoint == 0) {
			return;
		}
		
		int headingToPoint = headingToPoint(pointX, pointY);
		
		int changeOfHeading = getHeadingChange(headingToPoint);
		
		if(speed < movementSpeed) {
			slave.accelerate(movementSpeed);
		}
		
		if(speed > movementSpeed) {
			slave.decelerate();
		}
		
		if(changeOfHeading > 0) {
			slave.turnRight(changeOfHeading);
		}
		if(changeOfHeading < 0) {
			slave.turnLeft(changeOfHeading);
		}
    }
    
    private int getHeadingChange(int desiredHeading) {
    	int direction = slave.getEntity().getDirection();
    	int changeOfHeading = desiredHeading - direction;
		
		List<Integer> headingChoices = new ArrayList<>();
		headingChoices.add(Math.abs(changeOfHeading));
		headingChoices.add(Math.abs(changeOfHeading + 360));
		headingChoices.add(Math.abs(changeOfHeading - 360));
		
		int bestChoice = Collections.min(headingChoices);
		
		if(bestChoice == Math.abs(changeOfHeading + 360)) {
			changeOfHeading += 360;
		}
		
		if(bestChoice == Math.abs(changeOfHeading - 360)) {
			changeOfHeading -= 360;
		}
		
		return changeOfHeading;
    }
}

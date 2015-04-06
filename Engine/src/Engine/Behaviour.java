package Engine;
import javax.swing.JComponent;

public class Behaviour extends JComponent {

	private static final long serialVersionUID = 1L;

	protected Entity slave;
	
	public Entity getEntity() {
		return slave;
	}
	
	public void setEntity(Entity toSet) {
		slave = toSet;
	}
	
	public void moveUp() {
		int centerY = slave.getCenterY();
		
		centerY++;
		
		slave.setCenterY(centerY);
	}
	
    public void moveDown() {
    	int centerY = slave.getCenterY();
    	
    	centerY--;
    	
    	slave.setCenterY(centerY);
    }
    
    public void moveLeft() {
    	int centerX = slave.getCenterX();
    	
    	centerX--;
    	
    	slave.setCenterX(centerX);
    }
    
    public void moveRight() {
    	int centerX = slave.getCenterX();
    	
    	centerX++;
    	
    	slave.setCenterX(centerX);
    }
    
    public void turnLeft() {
    	int direction = slave.getDirection();
    	
    	direction--;
    	
    	slave.setDirection(direction);
    }
    
    public void turnLeft(int desiredDirection) {
    	int direction = slave.getDirection();
    	if(direction == desiredDirection) {
    		return;
    	}
    	
    	direction--;
    	
    	slave.setDirection(direction);
    }
    
    public void turnRight() {
    	int direction = slave.getDirection();
    	
    	direction++;
    	
    	slave.setDirection(direction);
    }
    
    public void turnRight(int desiredDirection) {
    	int direction = slave.getDirection();
    	if(direction == desiredDirection) {
    		return;
    	}
    	
    	direction++;
    	
    	slave.setDirection(direction);
    }
    
    public void accelerate() {
    	int speed = slave.getSpeed();
    	
    	speed++;
    	
    	slave.setSpeed(speed);
    }
    
    public void accelerate(int desiredSpeed) {
    	int speed = slave.getSpeed();
    	if(speed == desiredSpeed) {
    		return;
    	}
    	
    	speed++;
    	
    	slave.setDirection(speed);
    }
    
    public void decelerate() {
    	int speed = slave.getSpeed();
    	
    	speed--;
    	
    	slave.setSpeed(speed);
    }
    
    public void decelerate(int desiredSpeed) {
    	int speed = slave.getSpeed();
    	if(speed == desiredSpeed) {
    		return;
    	}
    	
    	speed--;
    	
    	slave.setDirection(speed);
    }
    
    public void stop() {
    	slave.setSpeed(0);
    }
}

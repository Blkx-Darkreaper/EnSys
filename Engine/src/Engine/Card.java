package Engine;
import java.awt.Color;
import java.awt.Graphics;
import java.awt.Point;
import java.awt.event.MouseEvent;
import java.awt.event.MouseListener;
import java.awt.event.MouseMotionListener;
import java.awt.image.BufferedImage;

import static Engine.Global.*;

import javax.swing.JComponent;
import javax.swing.JPanel;

public class Card extends JPanel {

	private boolean selected = false;
	private Point currentPoint;
	
	public Card () {
		super();

		setLocation(0, 0);
		setSize(250, 350);
		setBackground(Color.WHITE);
		repaint();
		//Graphics g = this.getGraphics();
		//g.drawImage(image, 0, 0, null);
		
		MouseListener clickListener = new MouseListener() {
			
			@Override
			public void mouseReleased(MouseEvent e) {
				selected = false;
			}
			
			@Override
			public void mousePressed(MouseEvent e) {
				selected = true;
				currentPoint = e.getPoint();
			}
			
			@Override
			public void mouseExited(MouseEvent e) {
				// TODO Auto-generated method stub
				
			}
			
			@Override
			public void mouseEntered(MouseEvent e) {
				// TODO Auto-generated method stub
				
			}
			
			@Override
			public void mouseClicked(MouseEvent e) {
				// TODO Auto-generated method stub
				
			}
		};
		MouseMotionListener movementListener = new MouseMotionListener() {
			
			@Override
			public void mouseMoved(MouseEvent e) {
				// TODO Auto-generated method stub
				
			}
			
			@Override
			public void mouseDragged(MouseEvent e) {
				Point endPoint = e.getPoint();
				
				int distanceX = endPoint.x - currentPoint.x;
				int distanceY = endPoint.y - currentPoint.y;
				
				Point currentPosition = getLocation();
				int endX = currentPosition.x + distanceX;
				int endY = currentPosition.y + distanceY;
				
				Point endPosition = new Point(endX, endY);
				setLocation(endPosition);
				repaint();
			}
		};
		addMouseListener(clickListener);
		addMouseMotionListener(movementListener);
	}
	
	@Override
	public void paintComponents(Graphics g) {
		super.paintComponents(g);
		
		//BufferedImage image = loadImage("test.png");
		//g.drawImage(image, 0, 0, null);
		g.setColor(Color.BLACK);
		g.drawRect(0, 0, 50, 70);
	}
}

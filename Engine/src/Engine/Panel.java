package Engine;
import java.awt.Color;
import java.awt.Dimension;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.image.BufferedImage;
import java.util.ArrayList;
import java.util.List;

import static Engine.Global.*;

import javax.swing.JPanel;

public class Panel extends JPanel {

	private static final long serialVersionUID = 1L;
	protected List<Environment> allEnvironments = new ArrayList<>();
	//protected List<Component> allControls = new ArrayList<>();
	
	public Panel(int inWidth, int inHeight, User user) {
		super();
		
		setVisible(true);
		setSize(inWidth, inHeight);
		Dimension size = new Dimension(inWidth, inHeight);
		setPreferredSize(size);
		String levelName = "germany";
		String tilesetName = "Germany";
		allEnvironments.add(new Environment(user, levelName, tilesetName, inWidth / 2, inHeight / 2, inWidth, 
				inHeight, this));
		//this.setBackground(Color.BLACK);
	}
	
	public void paintComponent(Graphics g) {
		super.paintComponent(g);
		Graphics2D g2d = (Graphics2D) g;
		
		for(Environment environment : allEnvironments) {
			BufferedImage image = environment.getCurrentImage();
			int scaledWidth = environment.getAbsoluteWidth();
			int scaledHeight = environment.getAbsoluteHeight();
			int absolutePositionX = environment.getAbsoluteCenterX() - scaledWidth / 2;
			int absolutePositionY = environment.getAbsoluteCenterY() - scaledHeight / 2;
			drawImage(g2d, image, absolutePositionX, absolutePositionY, scaledWidth, scaledHeight);
		}
		
		g2d.dispose();
	}
}

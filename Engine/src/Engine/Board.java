package Engine;
import java.awt.FlowLayout;
import java.awt.Font;
import java.awt.event.ActionEvent;
import java.awt.event.KeyEvent;
import java.awt.event.KeyListener;
import java.util.logging.Logger;

import static Engine.Global.*;

import javax.swing.Action;
import javax.swing.JFrame;

public class Board extends JFrame {
	
	private static final long serialVersionUID = 1L;
	protected int clientId;
	protected static int nextId = 1;
	protected User user;
	//protected int width, height;
	protected final int MARGIN_X = 6;
	protected final int MARGIN_Y = 31;
	
	public Board(String inTitle) {
		this(inTitle, 600, 400, DEFAULT_FONT, "");
	}
	
	public Board(String inTitle, int inWidth, int inHeight, Font inFont, String iconPath) {
		super();
		clientId = nextId;
		nextId++;
		user = new User(clientId);
		clientLogger = Logger.getLogger("Client log");
		//String logPath = "logs/client.log";
		//initLogging(clientLogger, clientFileHandler, logPath);

		setTitle(inTitle);
		
/*		int width = inWidth + MARGIN_X;
		int height = inHeight + MARGIN_Y;
		setSize(width, height);*/

		setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		setLayout(new FlowLayout());
		setLocationRelativeTo(null);
		setResizable(false);
		setVisible(true);
		setFocusable(true);
		setFont(inFont);
		//Image icon = loadImage(iconPath);
		//setIconImage(icon);
		
		//Panel panel = new Panel(300, 200);
		Panel panel = new Panel(600, 400, user);
		//add(panel);
		getContentPane().add(panel);
		
		Environment container = panel.allEnvironments.get(0);
		View selectable = (View) container.getComponent(3);
		View draggable = (View) container.getComponent(2);
		
		MouseDragListener dragListener = new MouseDragListener(draggable);
		addMouseListener(dragListener);
		addMouseMotionListener(dragListener);
		
		MouseSelectionListener selectionListener = new MouseSelectionListener(selectable);
		addMouseListener(selectionListener);
		addMouseMotionListener(selectionListener);

		pack();
		
		KeyListener keyListener = new KeyStrokeListener(user);
		addKeyListener(keyListener);
	}
	
	public User getUser() {
		return user;
	}

	public static void main(String[] args) {
		new Board("Test");
		
		MediaClip music = new MediaClip("background", "", "", "wav", true);
		MediaClip sound = new MediaClip("sound", "", "", "wav", false);
		music.play();
/*		sound.play();
		try {
			Thread.sleep(8000);
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		sound.play();*/
	}
}

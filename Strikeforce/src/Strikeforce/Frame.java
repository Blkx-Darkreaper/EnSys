package Strikeforce;

import javax.swing.*;
import static Strikeforce.Global.*;

public class Frame {
	
	public static void main(String[] args) {
		new Frame();
	}
	
	public Frame () {
		JFrame frame = new JFrame();
		frame.setTitle("Strikeforce");
		frame.add(new Board());
		frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		frame.setSize(FRAME_WIDTH, FRAME_HEIGHT);
		frame.setVisible(true);
		frame.setLocationRelativeTo(null);
	}
}

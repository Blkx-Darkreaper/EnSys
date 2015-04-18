package Strikeforce;

import static Strikeforce.Global.*;

import java.util.ArrayList;
import java.util.List;

public class Menu extends Entity {
	
	List<Entity> menuOptions = new ArrayList<>();

	public Menu(String inName, int inTitleHeight, List<String> inOptions) {
		super(inName, FRAME_WIDTH / 2 - VIEW_WIDTH / 2, FRAME_HEIGHT / 2, 0, 0);
		MENU_TITLE_HEIGHT = inTitleHeight;
		
		int menuSpacing = (MENU_HEIGHT - MENU_TITLE_HEIGHT) / inOptions.size();
		int offsetX = FRAME_WIDTH / 2 - VIEW_WIDTH / 2;
		int offsetY = (int) (FRAME_HEIGHT * 0.1) + MENU_HEIGHT - MENU_TITLE_HEIGHT;
		
		for(String menuOption : inOptions) {
			menuOptions.add(new Entity(inName + menuOption, offsetX, offsetY, 0, 0));
			offsetY -= menuSpacing;
		}
	}
	
	public List<Entity> getMenuOptions() {
		return menuOptions;
	}
}

package Engine;

import javax.swing.JComponent;

public class SelectableComponent extends JComponent {

	protected boolean isSelectable;
	
	public SelectableComponent() {
		this(false);
	}
	
	public SelectableComponent(boolean inCondition) {
		super();
		isSelectable = inCondition;
	}
	
	public boolean getIsSelectable() {
		return isSelectable;
	}
}

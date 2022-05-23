interface IEventListener
{
	void OnEvent(object e);
}

interface IEvents
{
	void Subscribe(IEventListener eventListener);
}
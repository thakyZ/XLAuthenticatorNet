using MaterialDesignThemes.Wpf.Transitions;
using XLAuth.Models.Abstracts;

namespace XLAuth.Domain.Components.Helpers;

/// <summary>
/// The slide navigator class
/// </summary>
internal sealed class SlideNavigator {
  /// <summary>
  /// The slide navigation subject
  /// </summary>
  private readonly ISlideNavigationSubject _slideNavigationSubject;
  /// <summary>
  /// The slides
  /// </summary>
  private readonly TransitionerSlide[] _slides;
  /// <summary>
  /// The history linked list
  /// </summary>
  private readonly LinkedList<SlideNavigatorFrame> _historyLinkedList = [];
  /// <summary>
  /// The current position node
  /// </summary>
  private LinkedListNode<SlideNavigatorFrame>? _currentPositionNode;

  /// <summary>
  /// Initializes a new instance of the <see cref="SlideNavigator"/> class
  /// </summary>
  /// <param name="slideNavigationSubject">The slide navigation subject</param>
  /// <param name="slides">The slides</param>
  internal SlideNavigator(ISlideNavigationSubject slideNavigationSubject, TransitionerSlide[] slides) {
    this._slideNavigationSubject = slideNavigationSubject;
    this._slides = slides;
  }

  /// <summary>
  /// Goes the to using the specified slide index
  /// </summary>
  /// <param name="slideIndex">The slide index</param>
  internal void GoTo(int slideIndex) => this.GoTo(slideIndex, () => { });

  /// <summary>
  /// Indexes the of slide
  /// </summary>
  /// <typeparam name="TSlide">The slide</typeparam>
  /// <returns>The int</returns>
  private int IndexOfSlide<TSlide>() => this._slides.Select((o, i) => new { o, i }).First(a => a.o is TransitionerSlide slide && slide.Content.GetType() == typeof(TSlide)).i;

  /// <summary>
  /// Goes the to
  /// </summary>
  /// <typeparam name="TSlide">The slide</typeparam>
  internal void GoTo<TSlide>() => this.GoTo(this.IndexOfSlide<TSlide>());

  /// <summary>
  /// Goes the to using the specified slide index
  /// </summary>
  /// <param name="slideIndex">The slide index</param>
  /// <param name="setupSlide">The setup slide</param>
  private void GoTo(int slideIndex, Action setupSlide) {
    if (this._currentPositionNode is null) {
      this._currentPositionNode = new LinkedListNode<SlideNavigatorFrame>(new SlideNavigatorFrame(slideIndex, setupSlide));
      this._historyLinkedList.AddLast(this._currentPositionNode);
    } else {
      LinkedListNode<SlideNavigatorFrame> newNode = new LinkedListNode<SlideNavigatorFrame>(new SlideNavigatorFrame(slideIndex, setupSlide));
      this._historyLinkedList.AddAfter(this._currentPositionNode, newNode);
      this._currentPositionNode = newNode;
      LinkedListNode<SlideNavigatorFrame>? tail = newNode.Next;
      while (tail is not null) {
        this._historyLinkedList.Remove(tail);
        tail = tail.Next;
      }
    }

    setupSlide();
    this.GoTo(this._currentPositionNode);
  }

  /// <summary>
  /// Goes the back
  /// </summary>
  internal void GoBack() {
    if (this._currentPositionNode?.Previous is null) {
      return;
    }

    this._currentPositionNode = this._currentPositionNode.Previous;
    this.GoTo(this._currentPositionNode);
  }

  /// <summary>
  /// Goes the forward
  /// </summary>
  internal void GoForward() {
    if (this._currentPositionNode?.Next is null) {
      return;
    }

    this._currentPositionNode = this._currentPositionNode.Next;
    this.GoTo(this._currentPositionNode);
  }

  /// <summary>
  /// Goes the to using the specified node
  /// </summary>
  /// <param name="node">The node</param>
  private void GoTo(LinkedListNode<SlideNavigatorFrame> node) => this._slideNavigationSubject.ActiveSlideIndex = node.Value.SlideIndex;
}

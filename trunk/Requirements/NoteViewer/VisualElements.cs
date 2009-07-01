using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Requirements.NoteViewer
{
    public class VisualElements : RequirementSet
    {
// ReSharper disable InconsistentNaming
        private Display Display;
        private NoteCatalog NoteCatalog;
        private Note Note;
// ReSharper restore InconsistentNaming

        public void SetupCollaborators()
        {
            var container = new Container();
            Display = container.Get<Display>();
            NoteCatalog = container.Get<NoteCatalog>();
            Note = container.Get<Note>();
        }

        public void WhenLoadingThePageACatalogOfNotesShouldAppearOnTheLeft()
        {
            When<View>(view => view.Load())
                .Should(() => Display.OnLeft(NoteCatalog));
        }

        public void WhenLoadingThePageTheCurrentlySelectedNoteShouldAppearOnTheRight()
        {
            When<View>(view => view.Load())
                .Should(() => Display.OnRight(Note));
        }

        public void WhenLoadingTheCatalogOfNotesTheFirstItemShouldBeSelected()
        {
            When<View>(view => view.Load())
                .Should(() => NoteCatalog.Select(items => items.First()));
        }

        public void WhenChangingTheSelectedItemInTheCatalogOfNotesTheCurrentlyViewedNoteShouldBeUpdated()
        {
            When<NoteCatalog>(catalog => catalog.Select(Any))
                .Should(catalog => Display.OnRight(catalog.SelectedNote));
        }
    }

    //I want to make this logic part of how the application executes, not just unit tests
    //Will end up having some sort of API for "GenerateObjectFromRequirements"

    public class Note
    {}

    public class NoteCatalog
    {
        public void Select(Func<IEnumerable<Note>, Note> notes)
        {}

        public Note SelectedNote { get; set; }
    }

    public class Display
    {
        public void OnLeft(object obj)
        {}

        public void OnRight(object obj)
        {}
    }

    public abstract class RequirementSet
    {
        public Requirement<TObject> When<TObject>(Expression<Action<TObject>> action)
        {
            return null;
        }

        public TItem Any<TItem>(IEnumerable<TItem> items)
        {
            return default(TItem);
        }
    }

    public class Requirement<TObject>
    {
        public void Should(Action<TObject> action)
        {}

        public void Should(Action action)
        {}
    }

    public class Container
    {
        public TObject Get<TObject>()
        {
            return default(TObject);
        }
    }

    public class View
    {
        public void Load()
        {}

        public void Display()
        {}
    }
}
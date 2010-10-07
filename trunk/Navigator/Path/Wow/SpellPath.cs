using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Navigator.UI.Attributes;

namespace Navigator.Path.Wow
{
    public class SpellCollectionPath : IHasRows<SpellPath>, ISummaryString
    {
        public SpellCollectionPath()
        {
            var d = new AhDownloader();
            d.Download();
        }

        public IEnumerable<SpellPath> Children
        {
            get
            {
                yield return new SpellPath {Name = "Shadow Bolt", Class = "Warlock"};
                yield return new SpellPath {Name = "Incinerate", Class = "Warlock"};
                yield return new SpellPath {Name = "Hammer of the Righteous", Class = "Paladin"};
                yield return new SpellPath {Name = "Hammer of the Righteous", Class = "Paladin"};
            }
        }

        public string Summary
        {
            get { return "Spells"; }
        }
    }

    public class TalentCollectionPath : IHasRows<TalentPath>, ISummaryString
    {
        public IEnumerable<TalentPath> Children
        {
            get
            {
                yield return new TalentPath
                                 {
                                     Name = "Butchery",
                                     Class = "Death Knight",
                                     Tree = "Blood",
                                     Requirements = "",
                                     NumberOfPoints = "2",
                                     Description = "Whenever you kill an enemy that grants experience or honor, you generate up to {0} runic power. In addition, you generate {1} runic power per 5 sec while in combat.",
                                     TalentFormulas = new List<Func<int, string>>{i => (i * 10).ToString(), i => i.ToString()}
                                 };
                yield return new TalentPath
                                 {
                                     Name = "Subversion",
                                     Class = "Death Knight",
                                     Tree = "Blood",
                                     Requirements = "",
                                     NumberOfPoints = "3",
                                     Description = "Increases the critical strike chance of Blood Strike, Scourge Strike, Heart Strike, Obliterate by {0}%, and reduces threat generated while in Blood or Unholy Presence by {1}%.",
                                     TalentFormulas = new List<Func<int, string>> {i => (i*3).ToString(), i => ((int) Math.Floor(8.4*i)).ToString()}
                                 };
            }
        }

        public string Summary
        {
            get { return "Talents"; }
        }
    }

    public class TalentPath : IHasColumns<TalentPath>
    {
        public IEnumerable<Expression<Func<TalentPath, string>>> Columns
        {
            get
            {
                yield return talentPath => talentPath.Class;
                yield return talentPath => talentPath.Tree;
                yield return talentPath => talentPath.Name;
                yield return talentPath => talentPath.NumberOfPoints;
                yield return talentPath => talentPath.Requirements;
            }
        }

        public string Name { get; set; }
        public string Class { get; set; }
        public string Tree { get; set; }
        public string Requirements { get; set; }
        public string NumberOfPoints { get; set; }
        public string Description { get; set; }
        public IEnumerable<Func<int, string>> TalentFormulas { get; set; }
    }

    public class SpellPath : IHasColumns<SpellPath>
    {
        public IEnumerable<Expression<Func<SpellPath, string>>> Columns
        {
            get
            {
                yield return spellPath => spellPath.Name;
                yield return spellPath => spellPath.Class;
            }
        }

        public string Name { get; set; }
        public string Class { get; set; }
    }

    public class ColumnInfo<TTarget>
    {
        public Func<TTarget, string> DataMethod { get; set; }
    }
}
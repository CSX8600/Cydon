using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Web.ViewEngine.ViewParts.Base;

namespace Web.ViewEngine.ViewParts
{
    public class Table : ViewPart
    {
        public override string OpeningTag => "table";
        public Table() : base()
        {
            Class.Add("table");

            Header = new TableSection(TableSection.SectionTypes.Header);
            Body = new TableSection(TableSection.SectionTypes.Body);
        }

        public TableSection Header { get; }
        public TableSection Body { get; }

        public override void RenderInnerContent(HtmlTextWriter writer)
        {
            Header.Render(writer);
            Body.Render(writer);
        }

        public class TableSection : ViewPart
        {
            public override string OpeningTag => SectionType == SectionTypes.Body ? "tbody" : "thead";

            public TableSection(SectionTypes sectionType) : base()
            {
                SectionType = sectionType;
            }

            public SectionTypes SectionType { get; }

            public List<Row> Rows { get; } = new List<Row>();

            public enum SectionTypes
            {
                Header,
                Body
            }

            public override void RenderInnerContent(HtmlTextWriter writer)
            {
                foreach(Row row in Rows)
                {
                    row.Render(writer);
                }
            }

            public Row CreateRow()
            {
                Row row = new Row(SectionType);
                Rows.Add(row);

                return row;
            }
        }

        public class Row : ViewPart
        {
            public override string OpeningTag => "tr";

            public Row(TableSection.SectionTypes sectionType) : base()
            {
                SectionType = sectionType;
            }

            public TableSection.SectionTypes SectionType { get; }

            public List<Column> Columns { get; } = new List<Column>();

            public override void RenderInnerContent(HtmlTextWriter writer)
            {
                foreach(Column column in Columns)
                {
                    column.Render(writer);
                }
            }

            public Column CreateColumn()
            {
                Column column = new Column(SectionType);
                Columns.Add(column);

                return column;
            }
        }

        public class Column : ViewPartContainer
        {
            public override string OpeningTag => SectionType == TableSection.SectionTypes.Header ? "th" : "td";

            public TableSection.SectionTypes SectionType { get; }
            public Column(TableSection.SectionTypes sectionType) : base()
            {
                SectionType = sectionType;
            }
        }
    }
}
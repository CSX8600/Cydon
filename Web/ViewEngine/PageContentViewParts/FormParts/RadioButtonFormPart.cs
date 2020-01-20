//using System;
//using System.Collections.Generic;
//using System.Xml.Linq;
//using Web.ViewEngine.ViewParts;
//using Web.ViewEngine.ViewParts.Base;

//namespace Web.ViewEngine.PageContentViewParts.FormParts
//{
//    public class RadioButtonFormPart : BaseFormPart
//    {
//        public string Question { get; set; }
//        public List<string> Answers { get; set; }
//        public override IEnumerable<ViewPart> GetEditableParts()
//        {
//            yield return new Header()
//            {
//                HeaderType = Header.HeaderTypes.H4,
//                Text = "Radio Button"
//            };

//            yield return BuildQuestionPartEditor();
//            yield return new Label()
//            {
//                Text = "Answers"
//            };

//            Div answersDiv = new Div()
//            {
//                Class = { "radio-group-answers" }
//            };

//            for(int i = 0; i < Answers.Count; i++)
//            {
//                answersDiv.Parts.Add(BuildAnswersPartEditor(Answers[i], i + 1));
//            }

//            Div answerTemplatedDiv = BuildAnswersPartEditor(null, 0);
//            answerTemplatedDiv.Class.Add("d-none");
//            answerTemplatedDiv.Class.Add("answer-template");
//            answersDiv.Parts.Add(answerTemplatedDiv);
//            yield return answersDiv;

//            yield return new Button()
//            {
//                Type = "button",
//                Class = { "btn", "btn-primary" },
//                ViewPartsBeforeText = new List<ViewPart>()
//                {
//                    new Span()
//                    {
//                        Class = { "fas", "fa-plus-circle" }
//                    }
//                },
//                OnClick = "formeditor.radiobutton.addQuestion($(this));",
//                Text = " Add Question",
//            };
//        }

//        public Div BuildQuestionPartEditor()
//        {
//            string id = GenerateID("question");
//            Div questionGroup = new Div()
//            {
//                Class = { "form-group" }
//            };

//            Label questionLabel = new Label()
//            {
//                InnerAttributes = { { "for", id } },
//                Text = "Question"
//            };

//            TextBox questionBox = new TextBox()
//            {
//                Id = id,
//                Class = { "form-control" },
//                Text = Question
//            };

//            Label questionValidation = new Label()
//            {
//                Class = { "text-danger" },
//                Data = { { "validate-message-for", id } }
//            };

//            questionGroup.Parts.AddRange(new List<ViewPart>() { questionLabel, questionBox, questionValidation });
//            return questionGroup;
//        }

//        public Div BuildAnswersPartEditor(string answer, int index)
//        {
//            Div row = new Div()
//            {
//                Class = { "form-row" }
//            };

//            Div answerDiv = new Div()
//            {
//                Class = { "form-group", "col-sm-11" }
//            };

//            TextBox answerBox = new TextBox()
//            {
//                Id = GenerateAnswerID(index),
//                Class = { "form-control" },
//                Text = answer
//            };

//            answerDiv.Parts.Add(answerBox);

//            Div deleteDiv = new Div()
//            {
//                Class = { "col-sm-1" }
//            };

//            Button delete = new Button()
//            {
//                Class = { "btn", "btn-danger" },
//                ViewPartsBeforeText = new List<ViewPart>()
//                {
//                    new Span()
//                    {
//                        Class = { "fas", "fa-trash" }
//                    }
//                },
//                OnClick = "formeditor.radiobutton.removeQuestion($(this));",
//                Type = "button"
//            };

//            deleteDiv.Parts.Add(delete);

//            row.Parts.Add(answerDiv);
//            row.Parts.Add(deleteDiv);

//            return row;
//        }

//        private string GenerateAnswerID(int index)
//        {
//            return GenerateID($"answer-{index}");
//        }

//        public override IEnumerable<ViewPart> GetNormalParts()
//        {
//            throw new NotImplementedException();
//        }

//        public override void ReadFromXML(XElement xElement)
//        {
//            Question = xElement.Attribute("question")?.Value;
//            Answers = new List<string>();

//            foreach(XElement answerElement in xElement.Elements())
//            {
//                if (answerElement.Name == "answer")
//                {
//                    Answers.Add(answerElement.Value);
//                }
//            }
//        }

//        public override void WriteToXML(XElement xElement)
//        {
//            xElement.Add(new XAttribute("question", Question));

//            foreach(string answer in Answers)
//            {
//                xElement.Add(new XElement("answer", answer));
//            }
//        }

//        public override void UpdateValuesFromForm(Dictionary<string, object> values)
//        {
//            Answers.Clear();
//            foreach(KeyValuePair<string, object> kvp in values)
//            {
//                string[] parts = kvp.Key.Split('-');
//                switch(parts[3])
//                {
//                    case "question":
//                        Question = kvp.Value as string;
//                        break;
//                    case "answer":
//                        if (parts[4] == "0")
//                        {
//                            continue;
//                        }

//                        if (parts.Length == 6 && parts[5] == "removed")
//                        {
//                            continue;
//                        }

//                        Answers.Add(kvp.Value as string);
//                        break;
//                }
//            }
//        }
//    }
//}
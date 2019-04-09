﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace WindowsFormsApplication1OOP
{
    public partial class Form1 : Form
    {
        public Form EForm;

        public Form1()
        {
            InitializeComponent();
        }

        public List<object> ObjectList = new List<object>()
        {
            new pilot()
            {name = "Алексей", SerialNumber = "P1"},
            new AirCraft()
            {SerialNumber = "AC0" },
            new Helicopter()
            {SerialNumber = "H0" },
            new MotorShip()
            {SerialNumber = "MS0" },
            new SailsShip()
            {SerialNumber = "SS0" },
            new Train()
            {SerialNumber = "T0" },
            new RacingCar()
            {SerialNumber = "RC0" },
            new CargoCar()
            {SerialNumber = "CC0" }
        };

        public List<MyCreator> MyCreatorList = new List<MyCreator>() {
            new AirCraftCreator(),
            new HelicopterCreator(),
            new SailsShipCreator(),
            new MotorShipCreator(),
            new TrainCreator(),
            new RacingCarCreator(),
            new CargoCarCreator(),
            new pilotCreator()
        };

        private void Form1_Load(object sender, EventArgs e)
        {
            ListView1.MultiSelect = false;
            // Создаем список выбора обьекта
            foreach (var creator in MyCreatorList)
                ObjectBox.Items.Add(creator.GetType().Name);
            ObjectBox.SelectedIndex = 0;
            ObjectBox.DropDownStyle = ComboBoxStyle.DropDownList;

            ListRedraw( ListView1, ObjectList);
        }

        private void Create_Click(object sender, EventArgs e)
        {
            
        }

        private void Edit_Click(object sender, EventArgs e)
        {
            //получаем индекc выделенного пункта
            int itemNum;
            if (ListView1.SelectedIndices.Count != 0)
                itemNum = ListView1.SelectedIndices[0];
            else
                return;

            //Получаем редактируемый объект
            object Obj = ObjectList[itemNum];

            //Получаем список всех полей объекта
            FieldInfo[] fields = Obj.GetType().GetFields(); ;

            //Создаем форму редактирования обьекта
            EForm = CreateForm(Obj, ObjectList);
            EForm.ShowDialog();
            EForm.Dispose();

            ListRedraw(ListView1, ObjectList);
        }

        //Форма редактирования обьекта
        private Form CreateForm(Object Obj, List<Object> ObjectList)
        {
            //список всех полей объекта
            FieldInfo[] fields = Obj.GetType().GetFields(); ;

            //создание пустой формы для редактирования полей
            Form form = new Form
            {
                Text = Obj.GetType().ToString(),
                Size = new System.Drawing.Size(400, 60 + 25 * (fields.Length + 2))
            };

            //создание полей
            for (int i = 0; i < fields.Length; i++)
            {
                //надпись содержащая тип и имя поля
                Label label = new Label
                {
                    Location = new Point(15, 25 * (i + 1)),
                    Width = string.Concat(fields[i].FieldType.Name, " ", fields[i].Name).Length * 7,
                    Text = string.Concat(fields[i].FieldType.Name, " ", fields[i].Name)
                };
                form.Controls.Add(label);

                //Создание для типов значений текстовых полей ввода и их заполнение
                if (((fields[i].FieldType.IsPrimitive) && (!fields[i].FieldType.IsEnum))
                  || (fields[i].FieldType == typeof(string)))
                {
                    TextBox text = new TextBox
                    {
                        Name = fields[i].Name,
                        Location = new Point(15 + label.Width, 25 * (i + 1)),
                        Width = form.Width - (label.Location.X + label.Width + 30),
                        Text = fields[i].GetValue(Obj).ToString()
                    };
                    form.Controls.Add(text);

                }//Создание выпадающих списков для перечислимых типов
                else if (fields[i].FieldType.IsEnum)
                {
                    ComboBox combobox = new ComboBox
                    {
                        Name = fields[i].Name,
                        SelectionStart = 0,
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        Location = new Point(15 + label.Width, 25 * (i + 1)),
                        Width = form.Width - (label.Location.X + label.Width + 30)
                    };
                    combobox.Items.AddRange(fields[i].FieldType.GetEnumNames());
                    combobox.SelectedIndex = (int)(fields[i].GetValue(Obj));
                    form.Controls.Add(combobox);

                }

                /*//Создание выпадающих списков для вложенных членов
                else if ((!fields[i].FieldType.IsPrimitive) && (!fields[i].FieldType.IsEnum) && (!(fields[i].FieldType == typeof(string))))
                {
                    ComboBox combobox = new ComboBox
                    {
                        Name = fields[i].Name,
                        SelectionStart = 0,
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        Location = new Point(15 + label.Width, 25 * (i + 1)),
                        Width = form.Width - (label.Location.X + label.Width + 30)
                    };

                    //список объектов удовлетворяющих типу поля
                    List<object> suitableItems = itemList.Where(sitem => ((sitem.GetType() == fields[i].FieldType) || (sitem.GetType().BaseType == fields[i].FieldType))).ToList();

                    for (int j = 0; j < suitableItems.Count; j++)
                    {
                        var nameField = suitableItems[0].GetType().GetField("name");
                        if (nameField != null)
                            combobox.Items.Add(nameField.GetValue(suitableItems[j]));
                    }

                    var buf = fields[i].GetValue(Obj);
                    int index = -1;

                    if (buf != null)
                    {
                        for (int j = 0; j < suitableItems.Count; j++)
                        {
                            if (buf.Equals(suitableItems[j]))
                            {
                                index = j; break;
                            }
                        }
                        combobox.SelectedIndex = index;
                    }

                    form.Controls.Add(combobox);
                }*/
            }

            //кнопка сохранения
            Button btn = new Button
            {
                Name = "btnSave",
                Text = "Save",
                Location = new Point(form.Width / 2 - (form.Width / 8), (fields.Length + 1) * 25),
                Width = form.Width / 4,
                DialogResult = DialogResult.OK,
            };

            //btn.Click += SaveControls;
            form.Controls.Add(btn);

            return form;
        }



        private void Delete_Click(object sender, EventArgs e)
        {

        }
        //Перерисовка списка объектов в соответсвии со списком обьектов
        public void ListRedraw(ListView listView, List<Object> ObjectList)
        {
            listView.Clear();
            for (int i = 0; i < ObjectList.Count; i++)
            {
                var listItem = new ListViewItem();
                Type ObjectType = ObjectList[i].GetType();
                object ObjSerialNumber;
                try
                {
                    var ObjSerialNumberField = ObjectList[i].GetType().GetField("SerialNumber");
                    ObjSerialNumber = ObjSerialNumberField.GetValue(ObjectList[i]);
                }
                catch
                {
                    ObjSerialNumber = "";
                }
                listItem.Text = ObjectType.Name + "  " + ObjSerialNumber;
                listView.Items.Add(listItem);
            }
        }


    }
}

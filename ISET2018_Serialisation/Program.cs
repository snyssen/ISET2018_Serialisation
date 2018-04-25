using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace ISET2018_Serialisation
{
    class Program
    {
        static void Main(string[] args)
        {
            #region PersonneSerialisee
            PersonneSerialisee ps = new PersonneSerialisee(1, "Winch", "Largo", new DateTime(1975, 07, 15));
            ps.Lst.Add("Danitza");
            ps.Lst.Add("Charity");
            ps.Lst.Add("Marilyn");
            ps.SerialiserFichier("essai.xml");
            PersonneSerialisee psBis = PersonneSerialisee.DeSerialiserFichier("essai.xml");
            Console.WriteLine(" {0} {1} ({2})", psBis.Prenom, psBis.Nom, psBis.ID);
            #endregion

            #region Personne
            Personne p = new Personne(1, "Winch", "Largo", new DateTime(1975, 07, 15));
            p.Lst.Add("Danitza");
            p.Lst.Add("Charity");
            p.Lst.Add("Marilyn");
            p.SerialiserFichier("essaiBis.xml");
            Personne pBis = Personne.DeSerialiserFichier("essaiBis.xml");
            Console.WriteLine(" {0} {1} ({2})", pBis.Prenom, pBis.Nom, pBis.ID);
            p.SerialiserToutFichier("EssaiTer.xml");
            Personne pTer = Personne.DeSerialiserToutFichier("EssaiTer.xml");
            Console.WriteLine(" {0} {1} ({2})", pTer.Prenom, pTer.Nom, pTer.ID);
            #endregion

            Console.ReadLine();
        }
    }
    [Serializable]
    [XmlRoot()]
    public class PersonneSerialisee
    {
        [XmlAttribute("Identifiant")]
        public int ID { get; set; }
        [XmlElement("Nom")]
        public string Nom { get; set; }
        [XmlElement("Prénom")]
        public string Prenom { get; set; }
        [XmlIgnore]
        public DateTime Naissance { get; set; }
        [XmlArray("Liste")]
        [XmlArrayItem("Conquête")]
        public List<string> Lst { get; set; }
        public PersonneSerialisee()
        {
            Lst = new List<string>();
        }
        public PersonneSerialisee(int ID_, string Nom_, string Prenom_, DateTime Naissance_) : this()
        {
            ID = ID_;
            Nom = Nom_;
            Prenom = Prenom_;
            Naissance = Naissance_;
        }

        public void SerialiserFichier(string nf)
        {
            using (StreamWriter sw = new StreamWriter(nf))
            {
                XmlSerializer xs = new XmlSerializer(this.GetType());
                xs.Serialize(sw, this);
                sw.Close();
            }
        }
        public static PersonneSerialisee DeSerialiserFichier(string nf)
        {
            using (StreamReader sr = new StreamReader(nf))
            {
                XmlSerializer xs = new XmlSerializer(typeof(PersonneSerialisee));
                PersonneSerialisee rep = (PersonneSerialisee)xs.Deserialize(sr);
                sr.Close();
                return rep;
            }
        }
    }
    public class Personne
    {
        public int ID { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public DateTime Naissance { get; set; }
        public List<string> Lst { get; set; }
        public Personne()
        {
            Lst = new List<string>();
        }
        public Personne(int ID_, string Nom_, string Prenom_, DateTime Naissance_) : this()
        {
            ID = ID_;
            Nom = Nom_;
            Prenom = Prenom_;
            Naissance = Naissance_;
        }
        public void SerialiserFichier(string nf)
        {
            using (XmlTextWriter xw = new XmlTextWriter(nf, Encoding.UTF8))
            {
                xw.WriteStartDocument();
                xw.WriteStartElement("Personne");
                xw.WriteAttributeString("Identifiant", ID.ToString());
                xw.WriteElementString("Prénom", Prenom);
                xw.WriteElementString("Nom", Nom);
                xw.WriteStartElement("Liste");
                foreach(string l in Lst)
                {
                    xw.WriteElementString("Conquête", l);
                }
                xw.WriteEndElement();
                xw.WriteEndElement();
                xw.WriteEndDocument();
                xw.Close();
            }
        }
        public static Personne DeSerialiserFichier(string nf)
        {
            Personne rep = new Personne();
            XmlTextReader xr = new XmlTextReader(nf);
            while (xr.Read())
            {
                if (xr.Name == "Personne")
                {
                    xr.MoveToAttribute("Identifiant");
                    rep.ID = xr.ReadContentAsInt();
                    xr.Read();
                    rep.Prenom = xr.ReadElementContentAsString();
                    rep.Nom = xr.ReadElementContentAsString();
                    if (xr.Name == "Liste" && !xr.IsEmptyElement)
                    {
                        xr.Read();
                        while (xr.Name == "Conquête")
                            rep.Lst.Add(xr.ReadElementContentAsString());
                    }
                    xr.Read();
                }
            }
            xr.Close();
            return rep;
        }
        public void SerialiserToutFichier(string nf)
        {
            XmlSerializer xs = new XmlSerializer(this.GetType());
            StreamWriter sw = new StreamWriter(nf);
            xs.Serialize(sw, this);
            sw.Close();
        }
        public static Personne DeSerialiserToutFichier(string nf)
        {
            XmlSerializer xs = new XmlSerializer(typeof(Personne));
            StreamReader sr = new StreamReader(nf);
            Personne rep = (Personne)xs.Deserialize(sr);
            sr.Close();
            return rep;
        }
    }
}
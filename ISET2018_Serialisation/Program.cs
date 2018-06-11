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

            #region Générique
            UtilitairesSerialisation.SerialiserFichier<Personne>("essai4.xml", p);
            Personne pQuat = UtilitairesSerialisation.DeSerialiserFichier<Personne>("essai4.xml");
            Console.WriteLine(" {0} {1} ({2})", pQuat.Prenom, pQuat.Nom, pQuat.ID);
            List<Personne> lPers = new List<Personne>();
            lPers.Add(p);
            lPers.Add(pBis);
            lPers.Add(pTer);
            lPers.Add(pQuat);
            UtilitairesSerialisation.SerialiserFichier<List<Personne>>("essai5.xml", lPers);
            #endregion

            Console.ReadLine();
        }
    }

    /// <summary>
    /// Ressort un xml de type :
    /// 
    ///     <?xml version="1.0" encoding="utf-8"?>
    ///     <PersonneSerialisee xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" Identifiant="1">
    ///       <Nom>Winch</Nom>
    ///       <Prénom>Largo</Prénom>
    ///       <Liste>
    ///         <Conquête>Danitza</Conquête>
    ///         <Conquête>Charity</Conquête>
    ///         <Conquête>Marilyn</Conquête>
    ///       </Liste>
    ///     </PersonneSerialisee>
    ///
    /// </summary>
    [Serializable] // On indique la classe comme sérialisable
    [XmlRoot()] // Indique "PersonneSerialisee" comme balise racine (=> celle qui encadre toute la classe) 
    public class PersonneSerialisee
    {
        [XmlAttribute("Identifiant")] // ajoute l'identifiant DANS la balise précédente (donc ici root => PersonneSerialisee)
        public int ID { get; set; }
        [XmlElement("Nom")] // Ajoute une balise "Nom" pour encadrer la variable Nom
        public string Nom { get; set; }
        [XmlElement("Prénom")] // idem
        public string Prenom { get; set; }
        [XmlIgnore] // Indique que la sérialisation doit ignorer cette variable => Elle ne sera pas sauvée dans le xml
        public DateTime Naissance { get; set; }
        [XmlArray("Liste")] // Indique une balise qui encadre un "bloc", ici notre liste de conquête
        [XmlArrayItem("Conquête")] // Indique que notre liste doit être sérialisée item par item dans le bloc "Liste", avec pour chaque item la balise "Conquête"
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

    /// <summary>
    /// Ressort un xml de type :
    /// SerialiserFichier (En une seule ligne dans le vrai fichier) ->
    /// 
    ///      <?xml version="1.0" encoding="utf-8"?>
    ///      <Personne Identifiant="1">
    ///          <Prénom>Largo</Prénom>
    ///          <Nom>Winch</Nom>
    ///          <Liste>
    ///              <Conquête>Danitza</Conquête>
    ///              <Conquête>Charity</Conquête>
    ///              <Conquête>Marilyn</Conquête>
    ///          </Liste>
    ///      </Personne>
    /// 
    /// SerialiserToutFichier ->
    /// 
    ///     <?xml version="1.0" encoding="utf-8"?>
    ///     <Personne xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
    ///       <ID>1</ID>
    ///       <Nom>Winch</Nom>
    ///       <Prenom>Largo</Prenom>
    ///       <Naissance>1975-07-15T00:00:00</Naissance>
    ///       <Lst>
    ///         <string>Danitza</string>
    ///         <string>Charity</string>
    ///         <string>Marilyn</string>
    ///       </Lst>
    ///     </Personne>
    /// 
    /// </summary>
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
            using (XmlTextWriter xw = new XmlTextWriter(nf, Encoding.UTF8)) // Utilisation d'un writer en xml
            {
                xw.WriteStartDocument(); // Démarre le doc -> Ecrit la version xml et son encodage (utf8)
                xw.WriteStartElement("Personne"); // Commence par la balise "Personne"... => StartElement écrit une balise qui va encadrer un bloc
                xw.WriteAttributeString("Identifiant", ID.ToString()); // ...A laquelle on attache l'ID
                xw.WriteElementString("Prénom", Prenom); // Balise "Prénom"
                xw.WriteElementString("Nom", Nom); // Balise "Nom"
                xw.WriteStartElement("Liste"); // A nouveau une balise 'bloc' => Encadre la liste
                foreach (string l in Lst)
                {
                    xw.WriteElementString("Conquête", l); // Eléments de la liste
                }
                xw.WriteEndElement(); // Ferme le bloc liste
                xw.WriteEndElement(); // Ferme le bloc "Personne"
                xw.WriteEndDocument(); // Ferme le document
                xw.Close(); // Ferme la "connexion" au document
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
            xs.Serialize(sw, this); // Sérialise toutes les données de la classe sans distinction, avec les noms des variables comme noms de balises
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

    /// <summary>
    /// Identique aux fonctions utilisées dans "Personne" mais générique pour être utilisable par n'importe quelle classe
    /// </summary>
    public class UtilitairesSerialisation
    {
        public static void SerialiserFichier<T>(string nf, T obj)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T)); // OU XmlSerializer xs = new XmlSerializer(obj.GetType());
            StreamWriter sw = new StreamWriter(nf);
            xs.Serialize(sw, obj);
            sw.Close();
        }
        public static T DeSerialiserFichier<T>(string nf)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            StreamReader sr = new StreamReader(nf);
            T rep = (T)xs.Deserialize(sr);
            sr.Close();
            return rep;
        }
    }

    /*
     *  POUR RESUMER :
     *  
     *  - PersonneSerialise utilise des instructions [] pour indiquer les balises xml des variables DANS LA CLASSE
     *  - Personne utilise :
     *                         1) une sérialisation "manuelle", élément par élément en indiquant nous même les balises et ce qu'il faut mettre entre elles
     *                         2) une sérialisation "automatique", qui prend toutes les variables de la classe et les insère dans le xml avec leurs noms comme balises
     *  - UtilitairesSerialisation permet d'utiliser la méthode automatique de Personne, mais de manière générique => Fonctionne pour n'importe quelle classe
     *
    */
}
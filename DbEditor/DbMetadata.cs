using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbEditor
{

    public static class DbMetadata
    {
        /// <summary>
        /// On create a User Accesory all metadata from the template is copied,
        /// for get the metadata in the describe combos must be present in the table combo_values 
        /// i.e.  is_hoody is metadata not shown in combos
        /// </summary>
        public static Dictionary<string, string[]> Classifiers = new Dictionary<string, string[]> { 
            {"beards", new string[] 
                {"ty_all", "ty_full", "ty_half", "ty_goatee", "ty_full_set", "ty_moustache" }},

            {"hats", new string[] 
                {"ty_all", "ty_african", "ty_bandanas", "ty_baseball", "ty_berets", "ty_caps", 
                 "ty_helmets", "ty_indians", "ty_misc", "ty_sports", "ty_meastern", "ty_windian",
                 "ty_brimmed", "ty_uniform", "ty_woollen",  "ty_femenine", "ty_jewish"}},
            
            {"hair", new string[] 
                {"gnd_all", "gnd_male", "gnd_female",
                 "eth_all", "eth_white", "eth_oriental", "eth_black", "eth_hispanic", "eth_indian" , "eth_meastern", "eth_eeuropean", 	
                 "len_all", "len_long", "len_shoulder", "len_medium", "len_short",  "len_vshort", "len_shaven" ,
                 "sty_all", "sty_tidy", "sty_untidy", "sty_spikey", "sty_parted", "sty_brback", "sty_brfwd", "sty_fringe", "sty_bouffant", "sty_punk", "sty_afro",
                 "ty_all", "ty_thining", "ty_wavy", "ty_curly", "ty_straight", "ty_afro", "ty_receding"  }} ,

            {"glasses", new string[] 
                { "ty_all", "ty_spectacles", "ty_sunglasses"}},

            {"ears", new string[]{}},
    
            {"clothing", new string[] 
                { "gnd_unsure", "gnd_male", "gnd_female", "ty_all", "ty_jumper", "ty_jacket", "ty_shirt",
                  "ty_polo_shirt", "ty_sport_shirt", "ty_top", "ty_t_shirt", "ty_uniform", "ty_hooded", "ty_dress",
                  "ty_hood_up", "ty_hood_down","ty_middle_eastern","ty_scarves","is_hoodie"}},
           
            {"logos", new string[] 
                { "ty_all", "ty_designer", "ty_corporate", "ty_sports","ty_sports_club"}},

            {"jewellery", new string[] 
                { "it_all", "it_earrings", "it_necklace", "it_piercings", 
                  "ty_all", "ty_e_studs", "ty_e_hoops", "ty_e_dangly", "ty_n_pendant",
                  "ty_n_chain", "ty_n_pearl", "ty_n_choker", "ty_n_bead", "ty_n_ornate"}},

            {"skineffects", new string[] 
                { "ty_stubble", "ty_nose", "ty_mouth", "ty_forehead", 
                  "ty_face", "ty_eyes", "ty_eyecolour", "ty_chin", "ty_cheeks"}},
        };
    }

}

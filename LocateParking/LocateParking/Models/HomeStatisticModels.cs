﻿
using Firebase.Database;
using Firebase.Database.Query;
using LocateParking.DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LocateParking.Models
{
    public class HomeStatisticModels
    {
        public List<HomeDTO> statisticList { get; set; }

        public static async System.Threading.Tasks.Task<HomeStatisticModels> Load(string sort = null)
        {
            var model = new HomeStatisticModels();
            model.statisticList = new List<HomeDTO>();

            FirebaseClient firebase = FirebaseSingleton.getInstance().getFirebaseClient();

            var statistics = await firebase
              .Child("statistic")
              .OrderByKey()
              .OnceAsync<Statistic>();

            foreach (var s in statistics)
            {
                model.statisticList.Add(await createHomeDTO(firebase, s.Object.userId,
                    s.Object.parkingId, s.Object.dateTime, s.Key));
            }

            return model;
        }

        public static async Task<HomeDTO> createHomeDTO(FirebaseClient firebase, String uId, String pId, String date, String sId)
        {
            var parking = await firebase.Child("parkings").OrderByKey().StartAt(pId).LimitToFirst(1).OnceAsync<Parking>();
            Parking p = parking.First().Object;

            var user = await firebase.Child("users").OrderByKey().StartAt(uId).LimitToFirst(1).OnceAsync<DTO.User>();
            DTO.User u = user.First().Object;

            //Debug.WriteLine("CREATE HOME DTO "+ messege);

            return new HomeDTO
            {
                statisticId = sId,
                dateTime = DateTime.Parse(date),
                userId = uId,
                userName = u.firstName + " " + u.lastName + "\n" + u.nickname,
                parkingId = pId,
                parkingName = p.name,
                parkingType = p.secret == "true" ? "Private" : "Public"
            };
        }
    }

}

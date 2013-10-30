﻿using System.Collections.Generic;
using System.Linq;
using System.Web;
using Flock.DTO;
using Flock.DataAccess.Base;
using Flock.DataAccess.EntityFramework;
using Flock.DataAccess.Repositories.Interfaces;
using Flock.Facade.Interfaces;
using System;


namespace Flock.Facade.Concrete
{
    public class QuackFacade : IQuackFacade
    {
        private readonly IQuackRepository _quackRepository;
        private readonly IQuackTypeRepository _quackTypeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IQuackLikeRepository _quackLikeRepository;
        private readonly IUserFacade _userFacade;

        public QuackFacade(IQuackRepository quackRepository, IQuackTypeRepository quackTypeRepository, IUserRepository userRepository, IQuackLikeRepository quackLikeRepository, IUserFacade userFacade)
        {
            _quackRepository = quackRepository;
            _quackTypeRepository = quackTypeRepository;
            _userRepository = userRepository;
            _quackLikeRepository = quackLikeRepository;
            _userFacade = userFacade;
        }


        public void SaveQuack(Quack quack)
        {


            quack.CreatedDate = DateTime.Now;
            quack.LastModifiedDate = DateTime.Now;
            quack.Active = true;
            var quackType = _quackTypeRepository.GetQuackByQuackType(quack.QuackTypeID);

            quack.QuackType = quackType;
            var user = _userRepository.GetUserById(quack.UserID);
            quack.User = user;

            quack.QuackContent.CreatedDate = DateTime.Now;
            _quackRepository.SaveQuack(quack);

        }

        public void GetQuack(int id)
        {
            _quackRepository.GetQuack(id);


        }
        public IList<QuackDto> GetQuacksInfo(int conversationId)
        {
            var quacks = _quackRepository.GetQuacksInfo(conversationId);
            var userName = HttpContext.Current.User.Identity.Name;
            var user = _userFacade.GetUserDetails(userName);

            var quacksInfo = (from quack in quacks let q = new QuackDto() select QuackMapper(quack, user.ID)).ToList();


            foreach (var q in quacksInfo )
             {
                 q.Replies = quacksInfo.Count - 1;
             }
            return quacksInfo;
        }

        private string VerifyLikeOrUnLike(Quack quack, int userId)
        {
            var check = quack.QuackLikes.FirstOrDefault(q => q.UserId == userId && q.Active && q.QuackId == quack.ID && q.Active);
            return check == null ? "Like" : "UnLike";
        }

        public IList<QuackDto> GetAllQuacks()
        {
            var userName = HttpContext.Current.User.Identity.Name;
            var user = _userFacade.GetUserDetails(userName);

            var quacks = _quackRepository.GetAllQuacks();
            var quackResults = new List<QuackDto>();

            foreach (var quack in quacks)
            {
                var q = new QuackDto();
                q = QuackMapper(quack, user.ID);
                var replies = _quackRepository.GetAllReplies(quack.ID);
                var qReplies = (from reply in replies let qreply = new QuackDto() select QuackMapper(reply)).ToList();
                q.QuackReplies = qReplies;
                q.Replies = replies.Count(qq => qq.Active);
                quackResults.Add(q);

            }

            return quackResults;
        }


        private QuackDto QuackMapper(Quack quack, int userId = 0)
        {
            return new QuackDto
                       {
                           Id = quack.ID,
                           Likes = quack.QuackLikes.Count(q => q.Active),
                           Message = quack.QuackContent.MessageText,
                           TimeSpan = GetTimeSpanInformation(quack.LastModifiedDate),
                           UserName = quack.User.UserName,
                           UserImage = quack.User.ProfileImage,
                           UserId = quack.User.ID,
                           LikeOrUnlike = VerifyLikeOrUnLike(quack, userId),
                           IsNew = quack.QuackTypeID == 1 ? true : false,
                           UserNickName = quack.User.UserName.Replace("DS\\", ""),
                           ConversationId = quack.ConversationID,
                           UserDisplayName = quack.User.FirstName + " "+quack.User.LastName
                       };
        }

        private string GetTimeSpanInformation(DateTime? d)
        {
            var result = "";
            if (d != null)
            {
                var date = (DateTime)d;
                TimeSpan timeSpan = DateTime.Now.Subtract(date);


                if (timeSpan.Days > 2)
                {
                    result = date.ToString("MMMM dd, yyyy") + " at " + date.ToString("hh:mm tt");
                }
                else if (timeSpan.Days > 0)
                {
                    result = timeSpan.Days + " days ago";
                }
                else if (timeSpan.Hours > 0)
                {
                    result = timeSpan.Hours + " hours ago";
                }
                else if (timeSpan.Minutes > 0)
                {
                    result = timeSpan.Minutes + " minutes ago";
                }
                else
                {
                    result = "Few seconds ago";
                }

            }
            return result;
        }

        public void DeleteQuack(int id)
        {
            _quackRepository.DeleteQuack(id);

            var conversations = _quackRepository.GetAllReplies(id);
            foreach (var conversation in conversations)
            {
                _quackRepository.DeleteQuack(conversation.ID);
            }


        }

        public void LikeOrUnlikeQuack(int quackId, int userId, Boolean isLike)
        {
            var quackLike = new QuackLike { QuackId = quackId, UserId = userId, Active = isLike };
            _quackLikeRepository.UpdateQuackLike(quackLike);
        }

    }
}
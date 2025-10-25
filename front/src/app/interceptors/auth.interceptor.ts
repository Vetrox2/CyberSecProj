import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { UserService } from '../services/user.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const userService = inject(UserService);
  const currentUserId = userService.currentUser()?.id;

  if (currentUserId) {
    const clonedRequest = req.clone({
      setHeaders: {
        'X-User-Id': currentUserId,
      },
    });
    return next(clonedRequest);
  }

  return next(req);
};

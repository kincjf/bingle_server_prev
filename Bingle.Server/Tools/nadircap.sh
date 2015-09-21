#!/bin/bash
#파노라마 사진에 미러볼 추가 script

#매개변수로 입력받은 원본파일 유무 검사
if [ -e "$1" ]; then

	#매개변수로 원본이미지 파일이름과과 변환된이미지 파일이름를 받는다
	#특수 변수 변경자인 :?로 $1, $2(첫번째, 두번째 매개변수)가 값을 가지고 있는지
	#검사하고 값이 없으면 스크립트를 종료하고 메시지를 출력한다
	input_image=${1:?"No source image name"}
	output_image=${2:?"No convert image name"}

	#임시저장할 미러볼 과 미러볼그림자효과 파일명 변수
	temp_nadir="temp_nadir_ball.png"
	temp_grad="temp_gradient.png"

	#이미지의 너비와 높이 값을 구해  변수에 저장
	get_image_width=$(identify -format "%w" $input_image)
	get_image_height=$(identify -format "%h" $input_image)

	#이미지 너비,높이, 미러볼크기, 미러볼그림자크기, 미러볼위치, 미러볼그림자위치 값설정
	image_height=$get_image_height
	image_width=$get_image_width

	nadircap_size=400	# nadircap 크기 조절
	
	gradient_size=`expr $nadircap_size / 2`
	nadirball_location=`expr $image_height - $nadircap_size`
	gradient_location=`expr $nadirball_location - $gradient_size / 2`

	echo $input_image $output_image
	echo $image_width $image_height
	echo $gradient_size $nadirball_location $gradient_location

	#미러볼 이미지 생성
	convert $input_image -crop $image_width'x'$image_height+0+0 -resize $image_width'x'$nadircap_size! -flip -level 0,130%% $temp_nadir

	#미러볼그림자 이미지 생성
	convert -size $image_width'x'$gradient_size gradient:none-black $temp_grad

	#원본사진에 미러볼과 미러볼그림자 이미지 추가
	convert $input_image -page +0+$gradient_location $temp_grad -flatten -page +0+$nadirball_location $temp_nadir -flatten $output_image
	
	#미러볼과 미러볼그림자 임시파일이 존재하면 삭제
	if [[ -e "$temp_nadir"  &&  -e "$temp_grad" ]]; then
		rm -f $temp_nadir
		rm -f $temp_grad
		echo 'Deleted temp file'
	else
		echo 'Not found temp file' >&2
		exit 1
	fi

else
	echo 'Not find source image' >&2
	exit 1
fi